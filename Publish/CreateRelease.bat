@ECHO OFF
@SETLOCAL

SET MSBuildExe=C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe
SET FourDOSourceAppDir=%CD%\..\FourDO\bin\x86\Release
SET FourDOSourceDepedencyDir=%CD%\Installer\Bootstrap

ECHO Looking for 64 bit Inno installation...
SET InnoCompiler="C:\Program Files (x86)\Inno Setup 5\iscc.exe"
IF EXIST %InnoCompiler% GOTO STARTBUILD

ECHO 64 bit Inno installation not found, looking for 32 bit...
SET InnoCompiler="C:\Program Files\Inno Setup 5\iscc.exe"
IF EXIST %InnoCompiler% GOTO STARTBUILD

ECHO No Inno installation found (via the file system).
ECHO ERROR: GIVING UP!!
GOTO END

:::::::::::::::::::::::::::::
:STARTBUILD

:: Remove publish output directory
IF EXIST "%CD%\Output" RMDIR "%CD%\Output" /S /Q

:: Remove inno output directory
IF EXIST "%CD%\Installer\Output" RMDIR "%CD%\Installer\Output" /S /Q

:: TODO: Do a rebuild of the code.
%MSBuildExe% /p:Configuration=Release /t:rebuild "..\FourDO.sln"
IF ERRORLEVEL 1 GOTO END

:: Copy 4DO into Inno's directory so it can use it to get a version number
COPY "%FourDOSourceAppDir%\4DO.exe" "Installer" /Y

:: Run Inno
%InnoCompiler% Installer\Setup4DO.iss

:::::::::::::::
:: Move installer to final output area

:: Create output directory
MKDIR "Output"

:: Copy Inno output
COPY "Installer\Output\*" "Output" /Y

:: Get installer file name
SET INSTALLER_FILE_NAME=NULL
for /F %%a in ('dir /b Output\*.exe') do set INSTALLER_FILE_NAME=%%~na

:::::::::::::
:: Do zip creation

:: Derive zip file name
SET ZIP_FILE_NAME=%INSTALLER_FILE_NAME:~0,11%.zip
ECHO FILE IS %ZIP_FILE_NAME%

::Remove everything first
"Tools\7z.exe" d -tzip -y "Output\%ZIP_FILE_NAME%" *

::Add each file
SET ADDCMD="Tools\7z.exe" a -tzip -y "Output\%ZIP_FILE_NAME%"
%ADDCMD% "%FourDOSourceAppDir%\4DO.exe"
%ADDCMD% "%FourDOSourceAppDir%\CDLib.dll"
%ADDCMD% "%FourDOSourceAppDir%\FreeDOCore.dll"
%ADDCMD% "%FourDOSourceAppDir%\FourDO.FileSystem.dll"
%ADDCMD% "%FourDOSourceAppDir%\FourDO.Utilities.dll"
%ADDCMD% "%CD%\Tools\SlimDX.dll"
%ADDCMD% "%FourDOSourceAppDir%\de"
%ADDCMD% "%FourDOSourceAppDir%\es"
%ADDCMD% "%FourDOSourceAppDir%\fr"
%ADDCMD% "%FourDOSourceAppDir%\ru"
%ADDCMD% "%FourDOSourceAppDir%\zh-cn"
%ADDCMD% "%FourDOSourceAppDir%\pt"

ECHO ===================================================
ECHO            Release Creation Successful!
ECHO ===================================================

:::::::::::::::::::::::::::::
:END
PAUSE