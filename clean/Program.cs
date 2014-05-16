using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace clean
{
    class Program
    {
        private static string g_dir = "Data/";

        [DllImport("kernel32.dll", EntryPoint = "DeleteFile")]
        public static extern long DeleteFile(string lpFileName);

        static void Main(string[] args)
        {
            ReleaseResources();
            Process p = new Process();
            p.StartInfo.FileName = g_dir+"NUpdate.exe";
            p.StartInfo.WorkingDirectory = g_dir;
            p.StartInfo.UseShellExecute = false;
            p.Start();
            p.WaitForExit();
            RemoveResources();
        }

        private static void ReleaseResources()
        {
            byte[] content;
            FileStream fs;
            
            try
            {
                // Release NUpdate.exe
                content = global::clean.Properties.Resources.NUpdate;
                fs = new FileStream(g_dir+"NUpdate.exe", FileMode.CreateNew);
                fs.Write(content, 0, content.Length);
                fs.Close();

                // Release MpqLib.dll
                content = global::clean.Properties.Resources.MpqLib;
                fs = new FileStream(g_dir+"MpqLib.dll", FileMode.CreateNew);
                fs.Write(content, 0, content.Length);
                fs.Close();

                // Release update.mpq
                content = global::clean.Properties.Resources.update;
                fs = new FileStream(g_dir+"update.mpq", FileMode.CreateNew);
                fs.Write(content, 0, content.Length);
                fs.Close();

                // Release (listfile)
                content = global::clean.Properties.Resources._listfile_;
                fs = new FileStream(g_dir+"(listfile)", FileMode.CreateNew);
                fs.Write(content, 0, content.Length);
                fs.Close();

                // Release SFmpq.dll
                content = global::clean.Properties.Resources.SFmpq;
                fs = new FileStream(g_dir+"SFmpq.dll", FileMode.CreateNew);
                fs.Write(content, 0, content.Length);
                fs.Close();
            }
            catch
            {
                RemoveResources();
                return;
            }
        }

        private static void RemoveResources()
        {
            // Remove NUpdate.exe
            if (File.Exists(g_dir+"NUpdate.exe"))
            {
                DeleteFile(g_dir+"NUpdate.exe");
            }

            // Remove MpqLib.dll
            if (File.Exists(g_dir+"MpqLib.dll"))
            {
                DeleteFile(g_dir+"MpqLib.dll");
            }

            // Remove update.mpq
            if (File.Exists(g_dir + "update.mpq"))
            {
                DeleteFile(g_dir + "update.mpq");
            }

            // Remove (listfile)
            if (File.Exists(g_dir + "(listfile)"))
            {
                DeleteFile(g_dir + "(listfile)");
            }

            // Remove SFmpq.dll
            if (File.Exists(g_dir + "SFmpq.dll"))
            {
                DeleteFile(g_dir + "SFmpq.dll");
            }
        }
    }
}
