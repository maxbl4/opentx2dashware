# DJI Log Toolbox
This program will convert OpenTx telemetry log CSV file to format better suitable for analysis and Dashware

Specifically it will:
- Add Timecode column with seconds from start of flight
- Add Timestamp field with sum of Date and Time
- Split GPS field to separate Lat and Long fields
- DistanceFromHome in meters
- DistanceTraveled in meters
- Convert ele/ail/thr/rud to percent
- Convert pitch/roll/yaw to degrees

## Building
- You need .Net 5.0 SDK installed. https://dotnet.microsoft.com/
- Run build.bat, it will produce .exe files in 'build' directory

## Usage
### fl-parser
Just run fl-parser.exe in the directory where you have OpenTx logs and DJI SRT files, it will try to convert every CSV and SRT it finds. It will also split individual flights from OpenTx log and merge SRT files from longer flights
### fl-merger
If you have OpenTx and DJI SRT logs from a single flight and you want to combine them, you first parse them with fl-parser, then supply OpenTx and DJI parsed CSV files to merger. It will create a single merged CSV with data from both logs

## Dashware
You can use included OpenTX.xml as starting point for Dashware profile. Copy it to ~\Documents\DashWare\DataProfiles

## NB
The included sample.csv is synthesized, this is not a real flight