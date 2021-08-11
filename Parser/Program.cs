using System;
using System.Linq;
using OpenTx2Dashware;

namespace Parser
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Parsing files....");
            var djiLogs = new DJISrtParser().Run().ToList();
            if (!(args.Length > 0 && int.TryParse(args[0], out var secondsBetweenFlights)))
                secondsBetweenFlights = 5;
            
            var openTxLogs = new OpenTxCsvParser{SecondsBetweenFlights = secondsBetweenFlights}.Run().ToList();
            Console.WriteLine("Writing output");
            foreach (var log in djiLogs.Concat(openTxLogs))
            {
                var fileName = $"{log.NamePrefix}_{log.StartTime:yyyy-MM-dd_HH-mm-ss}_parsed.csv";
                OpenTxCsvParser.WriteLog(fileName, log.Rows);
                Console.Write(".");
            }
            Console.WriteLine("\r\nDone");
        }
    }
}