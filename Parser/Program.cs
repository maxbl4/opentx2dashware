using System;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;
using OpenTx2Dashware;

namespace Parser
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Parsing files....");
            var djiLogs = new DJISrtParser().Run().ToList();
            var openTxLogs = new OpenTxCsvParser().Run().ToList();
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