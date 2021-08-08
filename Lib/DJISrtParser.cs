using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace OpenTx2Dashware
{
    public class DJISrtParser
    {
        public IEnumerable<Log> Run()
        {
            var files = Directory.GetFiles(".\\", "DJIG*.srt")
                .Where(x => !Path.GetFileNameWithoutExtension(x).EndsWith("_parsed"))
                .ToList();
            Console.WriteLine($"Found {files.Count} SRT files to process");
            var logs = files.Select(file =>
            {
                var log = new Log
                {
                    EndTime = File.GetLastWriteTime(file),
                    NamePrefix = Path.GetFileNameWithoutExtension(file)
                };
                using var sr = new StreamReader(file);
                while (true)
                {
                    sr.ReadLine();
                    var timestampString = sr.ReadLine();
                    var telemetryString = sr.ReadLine();
                    sr.ReadLine();

                    if (telemetryString == null)
                        break;
                    var ts = TimeSpan.Parse(timestampString.Substring(0, 12));
                    var fields = telemetryString
                        .Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(x => x.Split(':'))
                        .ToDictionary(x => x[0], x => x[1]);

                    log.Rows.Add(
                        new LogRow
                        {
                            Time = DateTime.MinValue + ts,
                            Timecode = int.Parse(fields["flightTime"]),
                            DJI_Signal = int.Parse(fields["signal"]), 
                            DJI_Channel = int.Parse(fields["ch"]), 
                            RxBattery = double.Parse(fields["uavBat"].Substring(0, fields["uavBat"].Length - 1),
                                CultureInfo.InvariantCulture), 
                            DJI_GoggleBattery = double.Parse(fields["glsBat"].Substring(0, fields["glsBat"].Length - 1),
                                CultureInfo.InvariantCulture), 
                            DJI_Delay = int.Parse(fields["delay"].Substring(0, fields["delay"].Length - 2)),
                            DJI_Bitrate = double.Parse(fields["bitrate"].Substring(0, fields["bitrate"].Length - 4),
                                CultureInfo.InvariantCulture),
                        });
                }

                log.UpdateTimestamps();
                Console.Write(".");
                return log;
            })
                .Where(x => x.Rows.Count > 0)
                .ToList();
            Console.WriteLine();
            var startLog = logs.FirstOrDefault();
            var combinedLogs = new List<Log>();
            if (startLog != null)
                combinedLogs.Add(startLog);
            for (var i = 1; i < logs.Count; i++)
            {
                var cur = logs[i];
                if (cur.Rows[0].Timecode < 5)
                {
                    startLog = cur;
                    combinedLogs.Add(cur);
                    continue;
                }
                startLog.Rows.AddRange(cur.Rows);
                startLog.EndTime = cur.EndTime;
                startLog.UpdateTimestamps();
            }

            return combinedLogs;
        }
    }
}