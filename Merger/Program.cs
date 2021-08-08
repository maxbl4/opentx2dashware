using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using OpenTx2Dashware;

namespace Merger
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Please supply two parsed CSV files to merge. One from OpenTx and One from DJI SRT");
                Console.ReadLine();
                return;
            }

            var log1 = OpenTxCsvParser.LoadRows(args[0], CultureInfo.CurrentCulture);
            var log2 = OpenTxCsvParser.LoadRows(args[1], CultureInfo.CurrentCulture);
            if (log1.Count < 1 || log2.Count < 1)
            {
                Console.WriteLine("One of the supplied logs is empty");
                Console.ReadLine();
                return;
            }

            string djiLogName;
            List<LogRow> openTxLog;
            List<LogRow> djiLog;
            if (log1[0].DJI_Bitrate > 0 && log2[0].DJI_Bitrate == 0)
            {
                djiLogName = args[0];
                openTxLog = log2;
                djiLog = log1;
            }else if (log1[0].DJI_Bitrate == 0 && log2[0].DJI_Bitrate > 1)
            {
                djiLogName = args[1];
                openTxLog = log1;
                djiLog = log2;
            }
            else
            {
                Console.WriteLine("You must supply one OpenTx log and one DJI log");
                Console.ReadLine();
                return;
            }

            foreach (var row in openTxLog)
            {
                var djiRow = djiLog.FirstOrDefault(x => x.Timecode >= row.Timecode);
                if (djiRow == null)
                    break;
                row.DJI_Channel = djiRow.DJI_Channel;
                row.DJI_Signal = djiRow.DJI_Signal;
                row.DJI_GoggleBattery = djiRow.DJI_GoggleBattery;
                row.DJI_Bitrate = djiRow.DJI_Bitrate;
                row.DJI_Delay = djiRow.DJI_Delay;
            }
            
            var namePrefix = djiLogName.Split('_')[0];
            var fileName = $"{namePrefix}_{djiLog[0].Timestamp:yyyy-MM-dd_HH-mm-ss}_merged.csv";
            OpenTxCsvParser.WriteLog(fileName, openTxLog);
        }
    }
}