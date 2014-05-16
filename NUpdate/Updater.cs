using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MpqLib;
using System.IO;
using System.Runtime.InteropServices;

namespace NUpdate
{
    class Updater
    {
        private string[] DEFAULT_IGNORE_LIST = new string[] { "(listfile)", "(attributes)", "(delete)", "(export)", "(version)", "(maindll)" };
        private Form1 _parent;
        private int steps;
        private bool doLogging;
        private string nirPath;
        private string sourceVersion;
        private string destVersion;

        public string[] deletelist;
        public string[] exportlist;
        public MpqLib.Mpq.CArchive PatchMPQ;
        public MpqLib.Mpq.CArchive TargetMPQ;

        [DllImport("kernel32.dll", EntryPoint = "DeleteFile")]
        public static extern long DeleteFile(string lpFileName);

        #region Encrypt related

        private string MpqPath_for_del_listfile;
        private const string sfmpq_dll = "SFmpq.dll";
        private const uint maxFilesInMpq = 1024;

        public const uint MOAU_CREATE_NEW = 0x00;
        public const uint MOAU_CREATE_ALWAYS = 0x08;
        public const uint MOAU_OPEN_EXISTING = 0x04;
        public const uint MOAU_OPEN_ALWAYS = 0x20;
        public const uint MOAU_READ_ONLY = 0x10;
        public const uint MOAU_MAINTAIN_LISTFILE = 0x01;

        [DllImport(sfmpq_dll, EntryPoint = "MpqOpenArchiveForUpdate")]
        public static extern int MpqOpenArchiveForUpdate(string lpFileName, uint dwFlags, uint dwMaximumFilesInArchive);
        [DllImport(sfmpq_dll, EntryPoint = "MpqCloseUpdatedArchive")]
        public static extern uint MpqCloseUpdatedArchive(int hMPQ, uint dwUnknown2);
        [DllImport(sfmpq_dll, EntryPoint = "MpqDeleteFile")]
        public static extern bool MpqDeleteFile(int hMPQ, string lpFileName);

        #endregion

        public Updater(Form1 parent, string patchMpqPath, string targetMpqPath, string gamePath, bool logging = true)
        {
            _parent = parent;
            doLogging = logging;
            nirPath = gamePath;

            PatchMPQ = new MpqLib.Mpq.CArchive(patchMpqPath);
            TargetMPQ = new MpqLib.Mpq.CArchive(targetMpqPath);
            MpqPath_for_del_listfile = targetMpqPath;

            // Import external listfile
            if (!TargetMPQ.FileExists("(listfile)"))
            {
                if (File.Exists("(listfile)")) TargetMPQ.ImportListFile("(listfile)");
                else return;
            }

            // Get version
            sourceVersion = "";
            destVersion = "";
            if (PatchMPQ.FileExists("(version)"))
            {
                var H_versionlist = new MpqLib.Mpq.CFileStream(PatchMPQ, "(version)");
                var versionlist_raw = new byte[H_versionlist.Length];
                versionlist_raw = H_versionlist.Read((int)H_versionlist.Length);
                H_versionlist.Close();
                string[] list = Encoding.ASCII.GetString(versionlist_raw).Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                destVersion = list[0];
                if (list.Length > 1) sourceVersion = list[1];
            }

            // initialize progressBar
            if (_parent != null)
            {
                steps = PatchMPQ.FileCount;
                deletelist = readDeleteList();
                steps += deletelist.Length;
                exportlist = readExportList();
                steps += exportlist.Length;
                _parent.setProgressBarMax(steps);
            }
        }

        ~Updater()
        {
            Close();
        }

        public void Close()
        {
            if (!TargetMPQ.IsDisposed)
            {
                TargetMPQ.Close();
                TargetMPQ.Dispose();
            }
            if (!PatchMPQ.IsDisposed)
            {
                PatchMPQ.Close();
                PatchMPQ.Dispose();
            }
        }
        
        // log to the front window
        private void log(string msg)
        {
            if (doLogging)
                _parent.log(msg);
            _parent.Refresh();
        }

        // set status to the front window
        private void setStatus(string msg)
        {
            if (doLogging)
                _parent.setStatus(msg);
            _parent.Refresh();
        }


        public string[] readDeleteList()
        {
            if (!PatchMPQ.FileExists("(delete)"))
                return new string[] { };
            var H_deletelist = new MpqLib.Mpq.CFileStream(PatchMPQ, "(delete)");
            var deletelist_raw = new byte[H_deletelist.Length];
            deletelist_raw = H_deletelist.Read((int)H_deletelist.Length);
            H_deletelist.Close();
            string[] list = Encoding.ASCII.GetString(deletelist_raw).Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            return list;
        }

        public void Delete(string filename)
        {
            try
            {
                setStatus(filename);
                TargetMPQ.RemoveFile(filename);
                //log("Deleted - " + filename);
            }
            catch (Exception e)
            {
                log("Error when deleting file from mpq - " + e.Message);
            }
            _parent.incProgressBar();
        }

        public void Delete(string[] filelist)
        {
            for (int i = 0; i < filelist.Length; i++)
            {
                Delete(filelist[i]);
            }
            
        }

        // Deletes all files from the deletelist
        public void DeleteAll()
        {
            if (deletelist.Length == 0)
                return;
            setStatus("Deleting Files...");
            log("[Start Deleting Files]");
            Delete(deletelist);
            log("[Finish Deleting Files]");
        }

        private void UpdateVersion()
        {
            File.WriteAllText("(version)", destVersion);
            TargetMPQ.ImportFile("(version)", "(version)");
            DeleteFile("(version)");

            string nirDll = nirPath + "Nirvana.dll";
            if (File.Exists(nirDll))
            {
                // read all bytes from Nirvana.dll
                byte[] content = File.ReadAllBytes(nirDll);

                string lookup = sourceVersion;
                if (lookup == "") lookup = "The Eve of Nirvana v0.08";
                string replace = destVersion;

                // replace version in content
                int forward = 0; // value is the same as position in string
                bool found = false;
                for (int i = 1; i < lookup.Length; i++)
                {
                    if (lookup[0] == lookup[i]) break;
                    forward++;
                }
                for (int i = 0; i < content.Length && !found; i++)
                {
                    if (content[i] == lookup[0])
                    {
                        int j = 1;
                        for (; j < lookup.Length; j++)
                        {
                            if (content[i + j] != lookup[j])
                            {
                                if (j <= forward) i += j - 1;
                                else i += forward;
                                break;
                            }
                        }
                        if (j == lookup.Length)
                        {
                            found = true;
                            for (int k = 0; k < destVersion.Length; k++)
                            {
                                content[i] = (byte)(destVersion[k]);
                                i++;
                            }
                            if (lookup.Length > destVersion.Length)
                            {
                                for (int k = 0; k < lookup.Length - destVersion.Length; k++)
                                {
                                    content[i] = 0x0;
                                    i++;
                                }
                            }
                        }
                    }
                }
                File.WriteAllBytes(nirDll, content);
            }
        }

        public void Flush() {
            setStatus("Flushing Changes...");
            try
            {
                UpdateVersion();
                TargetMPQ.Flush();
            }
            catch (Exception e)
            {
                log("Error when saving changes - " + e.Message);
            }
            log("[Flush MPQ] - Success!");
        }


        public void Compact()
        {
            setStatus("Compacting MPQ...");
            try
            {
                // Export listfile
                //TargetMPQ.ExportFile("(listfile)", "listfile.txt");

                TargetMPQ.Compact();
            }
            catch (Exception e)
            {
                log("Error when compacting mpq - " + e.Message);
            }
            log("[Compact MPQ] - Success!");
        }

        public void EncryptMpq()
        {
            // Delete listfile
            int hMPQ = MpqOpenArchiveForUpdate(MpqPath_for_del_listfile, MOAU_OPEN_EXISTING, maxFilesInMpq);
            MpqDeleteFile(hMPQ, "(listfile)");
            MpqCloseUpdatedArchive(hMPQ, 0);
        }

        public void Add(string filename, FileStream fs)
        {
            setStatus(filename);
            byte[] buffer = new byte[fs.Length];
            fs.Read(buffer, 0, (int)fs.Length);
            TargetMPQ.ImportFile(filename, buffer);
        }

        public void Add(string filename, MpqLib.Mpq.CFileStream cfs)
        {
            setStatus(filename);
            byte[] buffer = new byte[cfs.Length];
            cfs.Read(buffer, 0, (int)cfs.Length);
            TargetMPQ.ImportFile(filename, buffer);
        }

        public void Add(string filename, string realFilename)
        {
            FileStream fs = new FileStream(realFilename, FileMode.Open);
            Add(filename, fs);
            fs.Close();
        }

        // Adds all files from source mpq to target mpq
        public void AddAll(bool ignoreInternalFiles = true)
        {
            log("[Start Adding Files]");
            setStatus("Adding Files...");
            foreach (var file in PatchMPQ.FindFiles("*"))
            {
                // ignore internal files
                if (ignoreInternalFiles && DEFAULT_IGNORE_LIST.Contains(file.FileName))
                {
                    //log("Ignored - " + file.FileName);
                    _parent.incProgressBar();
                    continue;
                }

                // add files
                try
                {
                    var H_File = new MpqLib.Mpq.CFileStream(PatchMPQ, file.FileName);
                    Add(file.FileName, H_File);
                    //log("Added - " + file.FileName);
                }
                catch (Exception e)
                {
                    log("Error when adding file to mpq - " + e.Message);
                }
                _parent.incProgressBar();
            }
            log("[Finish Adding Files]");
        }

        public string[] readExportList()
        {
            if (!PatchMPQ.FileExists("(export)"))
                return new string[] {};
            var H_exportlist = new MpqLib.Mpq.CFileStream(PatchMPQ, "(export)");
            var exportlist_raw = new byte[H_exportlist.Length];
            exportlist_raw = H_exportlist.Read((int)H_exportlist.Length);
            H_exportlist.Close();
            string[] list = Encoding.ASCII.GetString(exportlist_raw).Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            return list;
        }

        public void Export(string line)
        {
            string seperateChar = "->";
            int pos = line.IndexOf(seperateChar);
            string srcPath = line.Substring(0, pos).Trim();
            string destPath = line.Substring(pos + seperateChar.Length).Trim();
            try
            {
                setStatus(destPath);
                PatchMPQ.ExportFile(srcPath, nirPath + destPath);
                PatchMPQ.RemoveFile(srcPath);
                //log("Exported - " + line);
            }
            catch (Exception e)
            {
                log("Error when exporting file from mpq - " + e.Message);
            }
            _parent.incProgressBar();
        }

        public void Export(string[] filelist)
        {
            for (int i = 0; i < filelist.Length; i++)
            {
                Export(filelist[i]);
            }

        }

        // Deletes all files from the deletelist
        public void ExportAll()
        {
            if (exportlist.Length == 0)
                return;
            setStatus("Exporting Files...");
            log("[Start Exporting Files]");
            Export(exportlist);
            log("[Finish Exporting Files]");
        }

        public string GetSourceVersion()
        {
            return sourceVersion;
        }

        public string GetDestVersion()
        {
            return destVersion;
        }
    }
}
