using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MpqLib;
using System.IO;

namespace NUpdate
{
    class Updater
    {
        private string[] DEFAULT_IGNORE_LIST = new string[] { "(listfile)", "(attributes)", "(delete)", "(export)", "(maindll)" };
        private Form1 _parent;
        private int steps;
        private bool doLogging;
        private string nirPath;

        public string[] deletelist;
        public string[] exportlist;
        public MpqLib.Mpq.CArchive PatchMPQ;
        public MpqLib.Mpq.CArchive TargetMPQ;

        public Updater(Form1 parent, string patchMpqPath, string targetMpqPath, string gamePath, bool logging = true)
        {
            _parent = parent;
            doLogging = logging;
            nirPath = gamePath;

            PatchMPQ = new MpqLib.Mpq.CArchive(patchMpqPath);
            TargetMPQ = new MpqLib.Mpq.CArchive(targetMpqPath);

            // initialize progressBar
            steps = PatchMPQ.FileCount;
            deletelist = readDeleteList();
            steps += deletelist.Length;
            exportlist = readExportList();
            steps += exportlist.Length;
            _parent.setProgressBarMax(steps);
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
                log("Deleted - " + filename);
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
            if (exportlist.Length == 0)
                return;
            setStatus("Deleting Files...");
            log("[Start Deleting Files]");
            Delete(deletelist);
            log("[Finish Deleting Files]");
        }


        public void Flush() {
            setStatus("Flushing Changes...");
            try
            {
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
                TargetMPQ.Compact();
            }
            catch (Exception e)
            {
                log("Error when compacting mpq - " + e.Message);
            }
            log("[Compact MPQ] - Success!");
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
            foreach (var File in PatchMPQ.FindFiles("*"))
            {
                // ignore internal files
                if (ignoreInternalFiles && DEFAULT_IGNORE_LIST.Contains(File.FileName))
                {
                    log("Ignored - " + File.FileName);
                    _parent.incProgressBar();
                    continue;
                }

                // add files
                try
                {
                    var H_File = new MpqLib.Mpq.CFileStream(PatchMPQ, File.FileName);
                    Add(File.FileName, H_File);
                    log("Added - " + File.FileName);
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
                log("Exported - " + line);
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


    }
}
