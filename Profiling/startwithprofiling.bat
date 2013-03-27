SETLOCAL
SET EXENAME=..\FourDO\bin\x86\Release\4DO.exe

rmdir output
mkdir output

set pt="C:\Program Files (x86)\Microsoft Visual Studio 10.0\Team Tools\Performance Tools"

:: Start the profiler
%pt%\vsperfcmd /start:sample /output:output\fourdo.vsp

:: Launch the application via the profiler
%pt%\vsperfcmd /launch:%EXENAME% /Timer:1000000

:: Shut down the profiler (this command waits, until the application is terminated)
%pt%\vsperfcmd /shutdown

:: generate the report files (.csv)
pushd output
%pt%\vsperfreport /summary:all fourdo.vsp 
popd