using System;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using Savage.GPS;
using Savage.Measurements.UnitsOfMeasure;

namespace OpenTx2Dashware
{
    class Program
    {
        static void Main(string[] args)
        {
            var files = Directory.GetFiles(".\\", "*.csv")
                .Where(x => !Path.GetFileNameWithoutExtension(x).EndsWith("_dashware"))
                .ToList();
            Console.WriteLine($"Found {files.Count} files to process");
            foreach (var file in files)
            {
                using var sr = new StreamReader(file);
                using var csvReader = new CsvReader(sr, new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HeaderValidated = null,
                    MissingFieldFound = null
                });
                var rows = csvReader.GetRecords<SourceRow>().ToList();
                if (rows.Count < 2)
                {
                    Console.WriteLine($"{Path.GetFileName(file)} does not have rows");
                    continue;
                }
                
                Console.WriteLine($"{Path.GetFileName(file)} has {rows.Count}");

                CsvWriter csvWriter = null;
                var flightIndex = 0;
                var startTimestamp = DateTime.MinValue;
                Position home = null;
                SourceRow prevRow = null;
                foreach (var row in rows)
                {
                    if (prevRow == null || (row.Timestamp - prevRow.Timestamp).TotalSeconds > 5)
                    {
                        flightIndex++;
                        if (csvWriter != null)
                            csvWriter.Dispose();
                        startTimestamp = row.Timestamp;
                        home = row.Position;
                        var outputName =
                            $"{Path.GetFileNameWithoutExtension(file)}_{flightIndex:00}_{row.Timestamp:yyyy-MM-dd_HH-mm-ss}_dashware.csv";
                        Console.WriteLine($"Creating output {outputName}");
                        csvWriter = new CsvWriter(new StreamWriter(outputName), 
                            new CsvConfiguration(CultureInfo.CurrentCulture));
                        csvWriter.WriteHeader<SourceRow>();
                        csvWriter.NextRecord();
                    }
                    else
                    {
                        row.DistanceTraveled = 
                            prevRow.DistanceTraveled + prevRow.Position.DistanceFrom(row.Position).Convert(Distances.Meters).Value;
                    }
                    
                    row.Timecode = (row.Timestamp - startTimestamp).TotalSeconds;
                    row.DistanceToHome = row.Position.DistanceFrom(home).Convert(Distances.Meters).Value;
                    row.Elevator = (row.Aileron + 1024) * 100 / 2048;
                    row.Aileron = (row.Aileron + 1024) * 100 / 2048;
                    row.Throttle = (row.Throttle + 1024) * 100 / 2048;
                    row.Rudder = (row.Aileron + 1024) * 100 / 2048;
                    row.Pitch = row.Pitch * 180 / Math.PI;
                    row.Roll = row.Roll * 180 / Math.PI;
                    row.Yaw = row.Yaw * 180 / Math.PI;
                    csvWriter.WriteRecord(row);
                    csvWriter.NextRecord();
                    prevRow = row;
                }
                csvWriter.Dispose();
            }
        }
    }

    class SourceRow
    {
        public double Timecode { get; set; }
        public DateTime Timestamp => Date + Time.TimeOfDay;
        public double Lat => double.Parse(GPS.SubstringSafe(0, 9) ?? "0", CultureInfo.InvariantCulture);
        public double Long => double.Parse(GPS.SubstringSafe(10, 9) ?? "0", CultureInfo.InvariantCulture);
        public Position Position => new(Long, Lat);
        public double DistanceToHome { get; set; } 
        public double DistanceTraveled { get; set; } 
        public DateTime Date { get; set; }
        public DateTime Time { get; set; }
        [Name("1RSS(dB)")]
        public int Rss1 { get; set; }
        [Name("RQly(%)")]
        public int RQly { get; set; }
        [Name("RSNR(dB)")]
        public int RSNR { get; set; }
        [Name("RFMD")]
        public int RFMD { get; set; }
        [Name("TRSS(dB)")]
        public int TRSS { get; set; }
        [Name("TQly(%)")]
        public int TQly { get; set; }
        [Name("TSNR(dB)")]
        public int TSNR { get; set; }
        [Name("RxBt(V)")]
        public double RxBattery { get; set; }
        [Name("Curr(A)")]
        public double Current { get; set; }
        [Name("Capa(mAh)")]
        public int Capacity { get; set; }
        [Name("Bat_(%)")]
        public int BatteryPercent { get; set; }
        [Name("Ptch(rad)")]
        public double Pitch { get; set; }
        [Name("Roll(rad)")]
        public double Roll { get; set; }
        [Name("Yaw(rad)")]
        public double Yaw { get; set; }
        [Name("GPS")]
        public string GPS { get; set; }
        [Name("GSpd(kmh)")]
        public double GpsSpeed { get; set; }
        [Name("Hdg(@)")]
        public double Heading { get; set; }
        [Name("Alt(m)")]
        public int Altitude { get; set; }
        [Name("Sats")]
        public int Sats { get; set; }
        [Name("Rud")]
        public int Rudder { get; set; }
        [Name("Ele")]
        public int Elevator { get; set; }
        [Name("Thr")]
        public int Throttle { get; set; }
        [Name("Ail")]
        public int Aileron { get; set; }
        [Name("TxBat(V)")]
        public double TxBattery { get; set; }
        [Name("SA")]
        public int SA { get; set; }
        [Name("SB")]
        public int SB { get; set; }
        [Name("SC")]
        public int SC { get; set; }
        [Name("SD")]
        public int SD { get; set; }
    }

    static class StringExt
    {
        public static string SubstringSafe(this string s, int start, int length)
        {
            try
            {
                return s.Substring(start, length);
            }
            catch
            {
                return null;
            }
        }
    }
}