using System;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

namespace taskmanager
{
    class Program
    {
        public static Dictionary<int, MyProcess> Processes = new Dictionary<int, MyProcess>();
        static void Main(string[] args)
        {
            while (true)
            {
                DisplayProcesses();
                UpdateProcesses();
                Thread.Sleep(1000);
            }
        }

        private static void DisplayProcesses()
        {
            Console.Clear();
            foreach (var kvp in Processes.OrderBy(p=>p.Value.CpuUsagePercent))
            {
                kvp.Value.StopCpuUsageMeasurement();
                Console.WriteLine(kvp.Value);
            }
        }

        private static void UpdateProcesses()
        {
            foreach (var process in Process.GetProcesses().OrderBy(p => p.ProcessName))
            {
                var myProcess = new MyProcess(process);
                if (myProcess.CanAccess())
                {
                    if (!Processes.ContainsKey(myProcess.Id))
                        Processes.Add(myProcess.Id, myProcess);

                    Processes[myProcess.Id].Update();
                }
            }
        }
    }
}