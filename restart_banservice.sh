#! /bin/bash

PROCESS=$(bash getbotprocess.sh BanService.dll)
kill -9 $PROCESS
printf "Killed BanService with pId $PROCESS\n"
cd mystuff/BanService/publish
nohup dotnet BanService.dll urls=https://localhost:5009 >/dev/null 2>&1 &
ps aux | grep dotnet