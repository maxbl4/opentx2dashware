using System;
using System.Linq;

namespace OpenTx2Dashware
{
    class Program
    {
        static void Main(string[] args)
        {
            var djiLogs = new DJISrtParser().Run().ToList();
            var openTxLogs = new OpenTxCsvParser().Run().ToList();
            Console.WriteLine($"DJI {djiLogs.Count}");
            foreach (var log in djiLogs)
            {
                Console.WriteLine($"{log.StartTime} {log.EndTime} {log.Duration} {log.Rows.Count} {log.DJIPrefix}");
            }
            Console.WriteLine($"OpenTX {openTxLogs.Count}");
            foreach (var log in openTxLogs)
            {
                Console.WriteLine($"{log.StartTime} {log.EndTime} {log.Duration} {log.Rows.Count}");
            }
        }
    }
}