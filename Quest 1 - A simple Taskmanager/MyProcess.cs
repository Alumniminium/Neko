using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace taskmanager
{
    public class MyProcess
    {
        public int Id;
        public string Name;
        public double CpuUsagePercent;
        public int MemoryUsageMBs;

        private TimeSpan _startCpuTime;
        private DateTime _startTime;

        public MyProcess(Process process)
        {
            Id = process.Id;
            Name = process.ProcessName;
        }
        public void StartCpuUsageMeasurement()
        {
            if(!CanAccess())
            return;
            _startTime = DateTime.UtcNow;
            _startCpuTime = Process.GetProcessById(Id).TotalProcessorTime;
        }
        public void StopCpuUsageMeasurement()
        {
            if(!CanAccess())
            return;
            var endTime = DateTime.UtcNow;
            var endCpuTime = Process.GetProcessById(Id).TotalProcessorTime;

            var cpuTimeDelta = (endCpuTime - _startCpuTime).TotalMilliseconds;
            var timeDelta = (endTime - _startTime).TotalMilliseconds;

            var cpuUsage = cpuTimeDelta / (Environment.ProcessorCount * timeDelta);
            CpuUsagePercent = Math.Round(cpuUsage * 100, 2);
        }

        public bool CanAccess()
        {
            try
            {
                var test = Process.GetProcessById(Id).TotalProcessorTime;
                var test2 = Process.GetProcessById(Id).WorkingSet64;
                return true;
            }
            catch { return false; }
        }

        private void GetRamUsage()
        {
            if(!CanAccess())
            return;
            var process = Process.GetProcessById(Id);
            MemoryUsageMBs = (int)process.WorkingSet64 / 1024 / 1024;
        }
        public void Update()
        {
            StartCpuUsageMeasurement();
            GetRamUsage();
        }

        public override string ToString()
        {
            return $"{$"{Id}".PadRight(8)} {Name.PadRight(32)} CPU: {CpuUsagePercent.ToString("00.00")}% \tMemory: {MemoryUsageMBs} MB";
        }
    }
}