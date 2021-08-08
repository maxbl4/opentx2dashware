using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenTx2Dashware
{
    public class Log
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public List<LogRow> Rows { get; set; } = new();
        public string NamePrefix { get; set; }
        public TimeSpan Duration => TimeSpan.FromSeconds(Rows.LastOrDefault()?.Timecode ?? 0);

        public void UpdateTimestamps()
        {
            var duration = Duration;
            StartTime = EndTime - Duration;
            foreach (var row in Rows)
            {
                row.Date = EndTime.Date;
                row.Time = EndTime - duration + TimeSpan.FromSeconds(row.Timecode);
            }
        }
    }
}