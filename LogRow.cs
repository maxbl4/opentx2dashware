using System;
using System.Globalization;
using CsvHelper.Configuration.Attributes;
using Savage.GPS;

namespace OpenTx2Dashware
{
    public class LogRow
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
        public int Delay { get; set; }
        public double Bitrate { get; set; }
    }
}