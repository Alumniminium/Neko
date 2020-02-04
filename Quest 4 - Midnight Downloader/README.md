# Requires
https://ytdl-org.github.io/youtube-dl/download.html
`youtube-dl.exe` has to be next to the app (bin/debug/netcoreapp/youtube-dl.exe) or inside %PATH%

On linux it just has to be installed w/ package manager and it will be inside $PATH by default.


There has to be a `Config.ini` file in the same directory as the `MMD.exe`
In case it's missing:
```ini
[MMD]
PlaylistFilePath=C:\DownloadList.txt
OutputPath=C:\out\
DownloadStartHour=2
DownloadEndTimeHour=8
```