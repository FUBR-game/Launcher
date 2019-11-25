using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;

namespace LoginScript
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Finding Processes");
            var launcherProcessId = 0;
            var launcherProcesses = Process.GetProcessesByName("dotnet");
            Console.WriteLine("Found " + launcherProcesses.Length + " Processes");
            foreach (var launcherProcess in launcherProcesses)
            {
                Console.WriteLine("Found process with title " + launcherProcess.MainWindowTitle);
                if (launcherProcess.MainWindowTitle == "FUBR Login")
                    launcherProcessId = launcherProcess.Id;
            }

            if (args.Length == 0 || launcherProcessId == 0)
            {
#if Linux
                Process.Start("./Launcher");
#endif
                return;
            }

            Console.WriteLine("Opening name pipe");
            var client = new NamedPipeClientStream("FubrLogin");
            Console.WriteLine("Connecting to Launcher");
            client.Connect();
            var writer = new StreamWriter(client);
            Console.WriteLine("writing to named pipe");

            writer.WriteLine(args[0].Substring(7));
            Console.WriteLine("flushing writer");
            writer.Flush();
            Console.WriteLine("closing writer");
            writer.Close();
            client.Close();
        }
    }
}