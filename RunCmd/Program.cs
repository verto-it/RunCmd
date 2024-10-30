using System.Diagnostics;

namespace RunCmd
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0 || args.Contains("--help") || args.Contains("--?"))
            {
                ShowHelp();
                return;
            }

            string scriptPath = null;
            string command = null;

            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "--script":

                        if (i + 1 < args.Length)
                        {
                            scriptPath = args[i + 1];
                        }
                        else
                        {
                            Console.WriteLine("Error: Missing script path after --script.");
                            return;
                        }
                        i++;
                        break;

                    case "--cmd":
                        if (i + 1 < args.Length)
                        {
                            command = args[i + 1];
                        }
                        else
                        {
                            Console.WriteLine("Error: Missing command after --cmd.");
                            return;
                        }
                        i++;
                        break;

                    default:
                        Console.WriteLine($"Unknown parameter: {args[i]}");
                        ShowHelp();
                        return;
                }
            }

            try
            {
                if (!string.IsNullOrEmpty(scriptPath))
                {
                    ExecuteScript(scriptPath);
                }
                else if (!string.IsNullOrEmpty(command))
                {
                    ExecuteCommand(command);
                }
                else
                {
                    Console.WriteLine("Error: No command or script specified.");
                    ShowHelp();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        static void ShowHelp()
        {
            Console.WriteLine("PortableExecutor - A tool to execute scripts or commands.\n");
            Console.WriteLine("Usage:");
            Console.WriteLine("  --script <path> : Path to the PowerShell or batch script to execute.");
            Console.WriteLine("  --cmd <command> : Command to execute.");
            Console.WriteLine("  --help or --?   : Display help information.\n");
            Console.WriteLine("Examples:");
            Console.WriteLine("  PortableExecutor --script \"C:\\path\\to\\script.ps1\"");
            Console.WriteLine("  PortableExecutor --cmd \"dir\"");
        }

        static void ExecuteScript(string scriptPath)
        {
            if (!File.Exists(scriptPath))
            {
                Console.WriteLine($"Error: Script file not found: {scriptPath}");
                return;
            }

            var extension = Path.GetExtension(scriptPath).ToLower();
            ProcessStartInfo psi;

            if (extension == ".ps1")
            {
                psi = new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Arguments = $"-ExecutionPolicy Bypass -File \"{scriptPath}\"",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                };
            }
            else if (extension == ".bat" || extension == ".cmd")
            {
                psi = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/C \"{scriptPath}\"",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                };
            }
            else
            {
                Console.WriteLine("Error: Unsupported script file type.");
                return;
            }

            using var process = Process.Start(psi);
            process.WaitForExit();
        }

        static void ExecuteCommand(string command)
        {
            var psi = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/C {command}",
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            using (var process = Process.Start(psi))
            {
                process.WaitForExit();
            }
        }
    }
}
