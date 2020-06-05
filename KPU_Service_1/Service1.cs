using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KPU_Service_1
{
    public partial class KPU_SERVICE : ServiceBase
    {
        private string filePathToObserve = "C:\\Windows\\Temp\\KPU\\";
        private string filePathToAddContent = "C:\\Users\\Jonathan\\Documents\\TEMP_KPU\\KPU_service_lab_main_file.txt";
        private bool _started;
        public KPU_SERVICE()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            _started = true;
            Task.Factory.StartNew(() =>
            {
                using (fileSystemWatcher1 = new FileSystemWatcher())
                {

                    if (!EventLog.SourceExists("KPU_SERVICE"))
                    {
                        EventLog.CreateEventSource("KPU_SERVICE", "Application");
                    }

                    LogEvent("KPU_SERVICE started " + DateTime.Now, EventLogEntryType.Information);

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
                    fileSystemWatcher1.Created += fileSystemWatcher1_Changed;
                    fileSystemWatcher1.Renamed += fileSystemWatcher1_Renamed;

                    // Begin watching.
                    fileSystemWatcher1.EnableRaisingEvents = true;


                    while (_started)
                    {
                        LogEvent("Service running", EventLogEntryType.Information);
                        Thread.Sleep(1000);
                    }

                }

                

            });



        }

        protected override void OnStop()
        {
            _started = false;
            LogEvent("Service stopped", EventLogEntryType.Information);
        }

        private void fileSystemWatcher1_Changed(object sender, FileSystemEventArgs e)
        {
            LogEvent("There has been created a file in the observed directory: " + e.FullPath, EventLogEntryType.Information);
            if (!File.Exists(e.FullPath)) return;

            var contentOfFile = File.ReadAllText(e.FullPath);

            File.WriteAllText(filePathToAddContent, contentOfFile);
            
        }

        private void fileSystemWatcher1_Renamed(object sender, FileSystemEventArgs e)
        {
            LogEvent("Renamed event " + e.FullPath, EventLogEntryType.Information);
        }

        private void LogEvent(string message, EventLogEntryType entryType)
        {
            EventLog eventLog = new EventLog {Source = "KPU_SERVICE", Log = "Application"};
            eventLog.WriteEntry(message, entryType);
        }

    }
}
