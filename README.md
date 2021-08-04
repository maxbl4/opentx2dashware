# opentx2dashware
This program will convert OpenTx telemetry log CSV file to format better suitable for Dashware

Specifically it will:
- Add Timecode column with seconds from start of flight
- Add Timestamp field with sum of Date and Time
- Split GPS field to separate Lat and Long fields
- DistanceFromHome in meters
- DistanceTraveled in meters
- Convert ele/ail/thr/rud to percent
- Convert pitch/roll/yaw to degrees

## Building
- You need .Net 5.0 SDK installed
- Run build.bat, it will produce .\build\OpenTx2Dashware.exe

##Usage
Just run OpenTx2Dashware.exe in the directory where you have OpenTx logs, it will try to convert every CSV it finds. It will also split individual flights from log, when there are 5+ seconds between log lines

##NB
The included sample.csv is synthesized, this is not a real flight