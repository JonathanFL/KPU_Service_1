using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp_test_service_logic
{
    class Program
    {
        private static string filePathToObserve = "C:\\Windows\\Temp\\KPU\\";
        private static string filePathToAddContent = "C:\\Users\\Jonathan\\Documents\\TEMP_KPU\\KPU_service_lab_main_file.txt";

        private static bool started;
        static void Main(string[] args)
        {
            using (FileSystemWatcher fileSystemWatcher1 = new FileSystemWatcher())
            {

                Console.WriteLine("Starting using FileSystemWatcher");
                started = true;
                Task t = Task.Factory.StartNew(() =>
                {
                    
                    fileSystemWatcher1.Path = filePathToObserve;

                    // Watch for changes in LastAccess and LastWrite times, and
                    // the renaming of files or directories.
                    fileSystemWatcher1.NotifyFilter = NotifyFilters.LastAccess
                                                      | NotifyFilters.LastWrite
                                                      | NotifyFilters.FileName
                                                      | NotifyFilters.DirectoryName;

                    // Only watch text files.
                    fileSystemWatcher1.Filter = "*.txt";

                    // Add event handlers.
                    fileSystemWatcher1.Created += FileSystemWatcher1Changed;
                    fileSystemWatcher1.Renamed += FileSystemWatcher1Renamed;

                    // Begin watching.
                    fileSystemWatcher1.EnableRaisingEvents = true;

                    var key = Console.ReadLine();
                    if (key.ToLower().Equals("stop"))
                    {
                        started = false;
                    }
                });

                while (started)
                {
                    
                }

                

                Console.WriteLine("End Using FileSystemWatcher");
                Console.ReadLine();
            }



            void FileSystemWatcher1Changed(object sender, FileSystemEventArgs e)
            {
                Console.WriteLine(e.Name + ", " + e.FullPath);
                if (!File.Exists(e.FullPath)) return;

                var contentOfFile = File.ReadAllText(e.FullPath);

                File.WriteAllText(filePathToAddContent, contentOfFile);
                File.WriteAllText("C:\\Users\\Jonathan\\Desktop\\KPU_SERVICE_DEBUG_FILE.txt", @"Came to end of fileSystemWatcher1_Changed");
            }

            void FileSystemWatcher1Renamed(object sender, FileSystemEventArgs e)
            {
                File.WriteAllText("C:\\Users\\Jonathan\\Desktop\\KPU_SERVICE_DEBUG_FILE.txt", @"fileSystemWatcher1_Renamed");
            }
        }
    }
}
