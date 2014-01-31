using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Win32;
using MpqLib;

using System.Windows.Forms;

namespace NUpdate
{
    class Utils
    {
        static private string resolvePath(string path)
        {
            if (path.Substring(path.Length - 1) != "\\")
                path += '\\';
            return path;
        }

        static private bool isNirvanaExist(string gamePath)
        {
            gamePath = resolvePath(gamePath);
            return File.Exists(gamePath + "Nirvana.mpq");
        }

        static public string getGamePath(string startPath="\\")
        {
            // Search current directory
            if (startPath == "\\" || startPath == "." || startPath == "" || startPath == null)
                startPath = Application.StartupPath;
            startPath = resolvePath(startPath);
            if (isNirvanaExist(startPath))
                return startPath;

            // Search registry
            RegistryKey pRegKey = Registry.CurrentUser.OpenSubKey(@"Software\Blizzard Entertainment\Warcraft III", true);
            if (pRegKey!=null)
            {
                string regPath = pRegKey.GetValue("InstallPath").ToString();
                if (isNirvanaExist(regPath))
                    return regPath;
            }
            return "";
        }

        static public string getGameVersion(string gamePath, string mpqName = "Nirvana.mpq")
        {
            gamePath = resolvePath(gamePath);
            string mpqPath = gamePath + mpqName;
            var Archive = new MpqLib.Mpq.CArchive(mpqPath);
            
            string version = "";
            try
            {
                var H_versionFile = new MpqLib.Mpq.CFileStream(Archive, "(version)");
                var version_raw = new byte[H_versionFile.Length];
                version_raw = H_versionFile.Read((int)H_versionFile.Length);
                H_versionFile.Close();
                version = Encoding.ASCII.GetString(version_raw);
            }
            catch
            {
            }
            Archive.Close();
            return version;
        }
    }
}
