#! /bin/bash
mkdir -p mystuff/BanService/publish
cd mystuff/BanService/publish
PROCESS=$(bash get_process.sh [.]/BanService)
kill -9 $PROCESS
printf "Killed BanService with pId $PROCESS\n"
printf "Enter Redis Connection String\n"
read RCS
export Redis=$RCS
nohup ./BanService urls=https://localhost:5009 >/dev/null 2>&1 &
ps aux | grep dotnet