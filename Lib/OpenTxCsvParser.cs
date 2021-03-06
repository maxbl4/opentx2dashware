using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;
using Savage.GPS;
using Savage.Measurements.UnitsOfMeasure;

namespace OpenTx2Dashware
{
    public class OpenTxCsvParser
    {
        public int SecondsBetweenFlights { get; set; } = 5;
        
        public IEnumerable<Log> Run()
        {
            var files = Directory.GetFiles(".\\", "*.csv")
                .Where(x => !Path.GetFileNameWithoutExtension(x).EndsWith("_parsed"))
                .Where(x => !Path.GetFileNameWithoutExtension(x).EndsWith("_merged"))
                .Where(x => !Path.GetFileNameWithoutExtension(x).StartsWith("DJIG"))
                .ToList();
            Console.WriteLine($"Found {files.Count} CSV files to process");
            foreach (var file in files)
            {
                var rows = LoadRows(file);
                if (rows.Count < 2)
                {
                    continue;
                }
                Log log = null;
                var flightIndex = 0;
                var startTimestamp = DateTime.MinValue;
                Position home = null;
                LogRow prevRow = null;
                foreach (var row in rows)
                {
                    if (prevRow == null || (row.Timestamp - prevRow.Timestamp).TotalSeconds > SecondsBetweenFlights)
                    {
                        Console.Write(".");
                        flightIndex++;
                        startTimestamp = row.Timestamp;
                        home = row.Position;
                        if (log != null)
                        {
                            log.EndTime = prevRow.Timestamp;
                            yield return log;
                        }

                        log = new Log
                        {
                            StartTime = startTimestamp,
                            NamePrefix = Path.GetFileNameWithoutExtension(file)
                        };
                    }
                    else
                    {
                        row.DistanceTraveled =
                            prevRow.DistanceTraveled + prevRow.Position.DistanceFrom(row.Position)
                                .Convert(Distances.Meters).Value;
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
                    log.Rows.Add(row);
                    prevRow = row;
                }

                if (log != null)
                {
                    log.EndTime = prevRow.Timestamp;
                    yield return log;
                }
            }
            Console.WriteLine();
        }

        public static List<LogRow> LoadRows(string file, CultureInfo cultureInfo = null)
        {
            cultureInfo ??= CultureInfo.InvariantCulture;
            try
            {
                using var sr = new StreamReader(file);
                using var csvReader = new CsvReader(sr, new CsvConfiguration(cultureInfo)
                {
                    HeaderValidated = null,
                    MissingFieldFound = null
                });
                return csvReader.GetRecords<LogRow>().ToList();
            }
            catch (Exception ex)
            {
                
            }

            return new List<LogRow>();
        }
        
        public static void WriteLog(string fileName, IEnumerable<LogRow> rows)
        {
            using var csvWriter = new CsvWriter(new StreamWriter(fileName), 
                new CsvConfiguration(CultureInfo.CurrentCulture));
            csvWriter.WriteHeader<LogRow>();
            csvWriter.NextRecord();
            foreach (var r in rows)
            {
                csvWriter.WriteRecord(r);
                csvWriter.NextRecord();
            }
        }

        public void ConvertAll()
        {
            var files = Directory.GetFiles(".\\", "*.csv")
                .Where(x => !Path.GetFileNameWithoutExtension(x).EndsWith("_parsed"))
                .Where(x => !Path.GetFileNameWithoutExtension(x).StartsWith("DJIG"))
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
                var rows = csvReader.GetRecords<LogRow>().ToList();
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
                LogRow prevRow = null;
                foreach (var row in rows)
                {
                    if (prevRow == null || (row.Timestamp - prevRow.Timestamp).TotalSeconds > SecondsBetweenFlights)
                    {
                        flightIndex++;
                        if (csvWriter != null)
                            csvWriter.Dispose();
                        startTimestamp = row.Timestamp;
                        home = row.Position;
                        var outputName =
                            $"{Path.GetFileNameWithoutExtension(file)}_{flightIndex:00}_{row.Timestamp:yyyy-MM-dd_HH-mm-ss}_parsed.csv";
                        Console.WriteLine($"Creating output {outputName}");
                        csvWriter = new CsvWriter(new StreamWriter(outputName), 
                            new CsvConfiguration(CultureInfo.CurrentCulture));
                        csvWriter.WriteHeader<LogRow>();
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
}