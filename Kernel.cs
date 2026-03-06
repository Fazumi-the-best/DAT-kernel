using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using Sys = Cosmos.System;
namespace DAT_src
{
    public class Kernel : Sys.Kernel
    {
        Sys.FileSystem.CosmosVFS fs = new Cosmos.System.FileSystem.CosmosVFS();
        string currentDir = @"0:\";
        FXRenderer renderer = new FXRenderer();
        protected override void BeforeRun()
        {
            Shell.CurrentDirectory = @"0:\";
            Console.Clear();
            // DONT ASK TWICE OS KERNEL COSMOS BASED
            Console.WriteLine("DAT-Kernel booted successfully.");
            Sys.FileSystem.VFS.VFSManager.RegisterVFS(fs);
            // hey twin dont uncomment the line below unless you know what you are doing it might delete all the disk data so dont be dumb :)
            //Sys.FileSystem.VFS.VFSManager.GetDisks()[0].FormatPartition(0, "FAT32");
            var available_space = fs.GetAvailableFreeSpace(@"0:\");
            Console.WriteLine("Available Free Space: " + available_space);
            var fs_type = fs.GetFileSystemType(@"0:\");
            Console.WriteLine("File System Type: " + fs_type);
        }
        protected override void Run()
        {
            // On start DAT will boot into the command shell.
            Shell.Run();
        }

        public static void cmdShutdown()
        {
            // shutdown OS
            Cosmos.System.Power.Shutdown();
        }
        public static void cmdReboot()
        {
            // reboot OS
            Cosmos.System.Power.Reboot();
        }
        public static void cmdEcho(string type, string text)
        {
            // echo text a certian amount of times
            int number = int.Parse(type);
            for (int i = 1; i <= number; i++)
            {
                Console.WriteLine(text);
            }
        }
        public static void cmdList(string dir)
        {
            // lists all the files and directorys, some bugs but i will fix them in a later release
            try
            {
                var files = Directory.GetFiles(dir);
                var dirs = Directory.GetDirectories(dir);
                foreach (var file in files)
                {
                    Console.Write("-");
                    Console.WriteLine(Path.GetFileName(file));
                }
                foreach (var d in dirs)
                {
                    Console.Write("*");
                    Console.WriteLine(Path.GetFileName(d));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error listing directory: " + e.Message);
            }
        }
        public static void cmdCreateFile(string currentDir, string filename)
        {
            // create a file
            if (string.IsNullOrWhiteSpace(filename))
            {
                Console.WriteLine("Missing file name");
                return;
            }
            try
            {
                string fullPath = Path.Combine(currentDir, filename);
                File.Create(fullPath).Close();
                Console.WriteLine("File created.");
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to create file: " + e.Message);
            }
        }
        public static void cmdWriteFile(string currentDir, string filename, string content)
        {
            // write text to a file
            if (string.IsNullOrWhiteSpace(filename))
            {
                Console.WriteLine("Missing file name");
                return;
            }
            string fullPath = Path.Combine(currentDir, filename);
            if (!File.Exists(fullPath))
            {
                Console.WriteLine("File not found :(");
                return;
            }
            try
            {
                File.WriteAllText(fullPath, content);
                Console.WriteLine("Written.");
            }
            catch (Exception e)
            {
                Console.WriteLine("Write failed: " + e.Message);
            }
        }
        public static void cmdReadFile(string currentDir, string filename)
        {
            // read the text content of a file

            if (string.IsNullOrWhiteSpace(filename))
            {
                Console.WriteLine("Missing file name");
                return;
            }
            string fullPath = Path.Combine(currentDir, filename);
            if (!File.Exists(fullPath))
            {
                Console.WriteLine("File not found :(");
                return;
            }
            try
            {
                Console.WriteLine(File.ReadAllText(fullPath));
            }
            catch (Exception e)
            {
                Console.WriteLine("Read failed: " + e.Message);
            }
        }
        public static void cmdReadBytes(string currentDir, string filename)
        {
            // read all the bytes of a file

            if (string.IsNullOrWhiteSpace(filename))
            {
                Console.WriteLine("Missing file name");
                return;
            }
            string fullPath = Path.Combine(currentDir, filename);
            if (!File.Exists(fullPath))
            {
                Console.WriteLine("File not found :(");
                return;
            }
            try
            {
                byte[] bytes = File.ReadAllBytes(fullPath);
                foreach (byte b in bytes)
                {
                    Console.Write(b + " ");
                }
                Console.WriteLine();
            }
            catch (Exception e)
            {
                Console.WriteLine("Read bytes failed: " + e.Message);
            }
        }
        public static void cmdCreateDirectory(string currentDir, string dirname)
        {
            // create a dir

            if (string.IsNullOrWhiteSpace(dirname))
            {
                Console.WriteLine("Missing directory name");
                return;
            }
            try
            {
                string fullPath = Path.Combine(currentDir, dirname);
                Directory.CreateDirectory(fullPath);
                Console.WriteLine("Directory created.");
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to create directory: " + e.Message);
            }
        }
        public static void cmdDeleteDirectory(string currentDir, string dirname)
        {
            // delete a dir
            if (string.IsNullOrWhiteSpace(dirname))
            {
                Console.WriteLine("Missing directory name");
                return;
            }
            string fullPath = Path.Combine(currentDir, dirname);
            if (!Directory.Exists(fullPath))
            {
                Console.WriteLine("Directory not found :(");
                return;
            }
            try
            {
                Directory.Delete(fullPath, true);
                Console.WriteLine("Directory deleted.");
            }
            catch (Exception e)
            {
                Console.WriteLine("Delete failed: " + e.Message);
            }
        }
        public static void cmdDeleteFile(string currentDir, string filename)
        {
            // delete a file

            if (string.IsNullOrWhiteSpace(filename))
            {
                Console.WriteLine("Missing file name");
                return;
            }
            string fullPath = Path.Combine(currentDir, filename);
            if (!File.Exists(fullPath))
            {
                Console.WriteLine("File not found :(");
                return;
            }
            try
            {
                File.Delete(fullPath);
                Console.WriteLine("File deleted.");
            }
            catch (Exception e)
            {
                Console.WriteLine("Delete failed: " + e.Message);
            }
        }
        public static string cmdChangeDirectory(string currentDir, string target)
        {
            // change current directory

            if (string.IsNullOrWhiteSpace(target))
            {
                Console.WriteLine("Missing directory name");
                return currentDir;
            }
            string newPath;
            if (target == @"\")
            {
                newPath = @"0:\";
            }
            else if (target == "..")
            {
                if (currentDir == @"0:\") return currentDir;
                string temp = currentDir.TrimEnd('\\');
                int lastSlash = temp.LastIndexOf('\\');
                if (lastSlash >= 2)
                {
                    newPath = temp.Substring(0, lastSlash + 1);
                }
                else
                {
                    newPath = @"0:\";
                }
            }
            else
            {
                newPath = Path.Combine(currentDir, target);
                if (!newPath.EndsWith(@"\")) newPath += @"\";
            }
            if (Directory.Exists(newPath))
            {
                return newPath;
            }
            else
            {
                Console.WriteLine("Directory not found :(");
                return currentDir;
            }
        }
        public static void cmdKillEverything()
        {
            // this does not work yet but if you can fix it then thanks

            try
            {
                foreach (var file in Directory.GetFiles(@"0:\"))
                {
                    File.Delete(file);
                }
                foreach (var dir in Directory.GetDirectories(@"0:\"))
                {
                    Directory.Delete(dir, true);
                }
                Console.WriteLine(".");
            }
            catch (Exception e)
            {
                Console.WriteLine(":( " + e.Message);
            }
        }

    }

}
