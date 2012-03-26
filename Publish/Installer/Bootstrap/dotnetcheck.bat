@ECHO OFF
@SETLOCAL

SET INSTALLER_EXECUTABLE=dotNetFx40_Client_setup.exe /passive

:: NOTE: I am looking for the existence of the "Install" flag only!
::       I have not tested whether or not this gets set to "0" on 
::       uninstall, but I highly doubt it!
::
::       http://msdn.microsoft.com/en-us/kb/kbarticle.aspx?id=318785

ECHO Now checking for a .NET 4 full installation
reg query "HKLM\SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full" /v Install
IF %ERRORLEVEL% EQU 0 GOTO NOINSTALL

ECHO Now checking for a .NET 4 client installation
reg query "HKLM\SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Client" /v Install
IF %ERRORLEVEL% EQU 0 GOTO NOINSTALL

:: If we got here, we need to install it!
:: NOTE: This assumes the existence of the installer in the same directory!
ECHO .NET 4 Framework not found. Starting installation now.
%INSTALLER_EXECUTABLE%
GOTO END

:::::::::::::::::::::::::::::::::::::::::::::::::
:NOINSTALL
ECHO No installation necessary!

GOTO END

:::::::::::::::::::::::::::::::::::::::::::::::::
:END