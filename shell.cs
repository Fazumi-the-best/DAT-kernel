using System;
using System.IO;
using Sys = Cosmos.System;

namespace FX_src
{
    public static class Shell
    {
        public static string CurrentDirectory { get; set; } = @"0:\";

        public static void Run()
        {
            while (true)
            {
                Console.Write(CurrentDirectory + " ");
                string input = Console.ReadLine()?.Trim();

                if (string.IsNullOrWhiteSpace(input))
                    continue;

                // Basic split – first word = command, rest = arguments
                string[] parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 0) continue;

                string cmd = parts[0].ToLowerInvariant();
                string arg1 = parts.Length > 1 ? parts[1] : "";
                string rest = "";

                // Take everything after command + first arg (good for write/echo messages)
                if (parts.Length > 2)
                {
                    int firstSpace = input.IndexOf(' ');
                    int secondSpace = input.IndexOf(' ', firstSpace + 1);
                    if (secondSpace > firstSpace)
                        rest = input.Substring(secondSpace + 1);
                }

                ExecuteCommand(cmd, arg1, rest);
            }
        }

        private static void ExecuteCommand(string cmd, string arg1, string rest)
        {
            switch (cmd)
            {
                case "list":
                    Kernel.cmdList(CurrentDirectory);
                    break;

                case "create":
                    Kernel.cmdCreateFile(CurrentDirectory, arg1);
                    break;

                case "write":
                    Kernel.cmdWriteFile(CurrentDirectory, arg1, rest);
                    break;

                case "echo":
                    Kernel.cmdEcho(arg1, rest);
                    break;

                case "read":
                    Kernel.cmdReadFile(CurrentDirectory, arg1);
                    break;

                case "read_bytes":
                    Kernel.cmdReadBytes(CurrentDirectory, arg1);
                    break;

                case "create_dir":
                case "mkdir":
                    Kernel.cmdCreateDirectory(CurrentDirectory, arg1);
                    break;

                case "delete_dir":
                case "rmdir":
                    Kernel.cmdDeleteDirectory(CurrentDirectory, arg1);
                    break;

                case "delete_file":
                case "del":
                case "rm":
                    Kernel.cmdDeleteFile(CurrentDirectory, arg1);
                    break;

                case "shutdown":
                    Kernel.cmdShutdown();
                    break;

                case "reboot":
                    Kernel.cmdReboot();
                    break;

                case "cd":
                    CurrentDirectory = Kernel.cmdChangeDirectory(CurrentDirectory, arg1);
                    break;

                case "kill":
                    Kernel.cmdKillEverything();
                    break;

                default:
                    Console.WriteLine($"Unknown command: {cmd}");
                    break;
            }
        }
    }
}