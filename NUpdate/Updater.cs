using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MpqLib;
using System.IO;
using System.Windows.Forms;

namespace NUpdate
{
    class Updater
    {
        private string[] DEFAULT_IGNORE_LIST = new string[] { "(listfile)", "(attributes)", "(version)", "(delete)" };
        private Form1 _parent;
        private int steps;
        public string[] deletelist;
        public MpqLib.Mpq.CArchive PatchMPQ;
        public MpqLib.Mpq.CArchive TargetMPQ;
        public Updater(Form1 parent, string patchMpqPath, string targetMpqPatch)
        {
            _parent = parent;
            PatchMPQ = new MpqLib.Mpq.CArchive(patchMpqPath);
            TargetMPQ = new MpqLib.Mpq.CArchive(targetMpqPatch);

            // initialize progressBar
            steps = PatchMPQ.FileCount;
            deletelist = readDeleteList();
            steps += deletelist.Length;
            _parent.setProgressBarMax(steps);
        }

        ~Updater()
        {
            Close();
        }

        public void Close()
        {
            TargetMPQ.Close();
            PatchMPQ.Close();
        }

        public string[] readDeleteList()
        {
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
                TargetMPQ.RemoveFile(filename);
                _parent.log("Delete - " + filename);
            }
            catch (Exception e)
            {
                _parent.log("Error when deleting file from mpq - " + e.Message);
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

        public void Flush() { TargetMPQ.Flush(); }

        public void Compact() { TargetMPQ.Compact(); }

        public void Add(string filename, FileStream fs)
        {
            byte[] buffer = new byte[fs.Length];
            fs.Read(buffer, 0, (int)fs.Length);
            TargetMPQ.ImportFile(filename, buffer);
        }

        public void Add(string filename, string realFilename)
        {
            FileStream fs = new FileStream(realFilename, FileMode.Open);
            Add(filename, fs);
            fs.Close();
        }

        public void AddAll(bool ignoreInternalFiles = true)
        {
            foreach (var File in PatchMPQ.FindFiles("*"))
            {
                Console.WriteLine(File.FileName);
                //Do something with File.FileName here
                _parent.incProgressBar();
            }
        }
    }
}
