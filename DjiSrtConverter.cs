using System;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;

namespace OpenTx2Dashware
{
    public class DJISrtConverter
    {
        public void Run()
        {
            var files = Directory.GetFiles(".\\", "DJIG*.srt").ToList();
            Console.WriteLine($"Found {files.Count} SRT files to process");
            foreach (var file in files)
            {
                using var csvWriter = new CsvWriter(new StreamWriter(Path.GetFileNameWithoutExtension(file) + ".csv"), 
                    new CsvConfiguration(CultureInfo.CurrentCulture));
                csvWriter.WriteHeader<DJITelemetry>();
                csvWriter.NextRecord();
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
                    csvWriter.WriteRecord(
                        new DJITelemetry
                        {
                            Timestamp = ts,
                            FlightTime = int.Parse(fields["flightTime"]),
                            Delay = int.Parse(fields["delay"].Substring(0, fields["delay"].Length - 2)),
                            Bitrate = double.Parse(fields["bitrate"].Substring(0, fields["bitrate"].Length - 4),
                                CultureInfo.InvariantCulture),
                        });
                    csvWriter.NextRecord();
                }
            }
        }
    }

    public class DJITelemetry
    {
        public TimeSpan Timestamp { get; set; }
        public int FlightTime { get; set; }
        public int Delay { get; set; }
        public double Bitrate { get; set; }
    }
}