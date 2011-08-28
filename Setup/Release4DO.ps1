##########################################

$ErrorActionPreference = 'Stop';
$executingScriptDirectory = Split-Path -Path $MyInvocation.MyCommand.Definition -Parent

$projectRoot = (Resolve-Path -LiteralPath (Join-Path $executingScriptDirectory '..')).Path;

$bootStrapFolder = "BootStrapped";
$finalFolder = "Final";

##############
# Function to replace a string in a file and output the new file somewhere else.
##############
function Replace-InFile
{
	param
	(
		[string]$inFileName,
		[string]$outFileName,
		[string]$textToFind,
		[string]$textToInsert
	);
	
	(Get-Content $inFileName) | `
		Foreach-Object {$_ -replace $textToFind, $textToInsert} | `
		Set-Content $outFileName
}

######################################################
# MAIN
######################################################

# Clean any existing files.
$pathToClean = (Join-Path $projectRoot "Setup\$bootStrapFolder");
if (Test-Path $pathToClean) 
{
	Remove-Item $pathToClean -Recurse -Force;
}
$pathToClean = (Join-Path $projectRoot "Setup\$finalFolder");
if (Test-Path $pathToClean) 
{
	Remove-Item $pathToClean -Recurse -Force;
}

# Find MSBuild location.
$msBuild = (Get-ItemProperty 'HKLM:\SOFTWARE\Microsoft\MSBuild\ToolsVersions\4.0').MSBuildToolsPath;
$msBuild = $msBuild + 'MSBuild.exe';

# Build the base installer (and the encapsulating bootstrap)
& $msBuild /t:rebuild /p:Configuration=Package /p:Platform="x86" "$projectRoot\FourDO.sln";
& $msBuild /t:rebuild /p:Configuration=Package /p:Platform="x64" "$projectRoot\FourDO.sln";

# Get 4DO version.
$releaseVersion = [System.Diagnostics.FileVersionInfo]::GetVersionInfo("$projectRoot\FourDO\bin\x86\Release\4DO.exe").FileVersion

# Copy 4DOSetup into bootstrap folder (why the hell doesn't it do this?)
Copy-Item "$projectRoot\WixSetup\bin\x86\Release\4DOSetup.msi" "$projectRoot\Setup\$bootStrapFolder\x86" -Force;
Copy-Item "$projectRoot\WixSetup\bin\x64\Release\4DOSetup.msi" "$projectRoot\Setup\$bootStrapFolder\x64" -Force;

# Create temporary SED files with the correct version numbers as the target.
Replace-InFile "$projectRoot\Setup\4DO_x86.SED" "$projectRoot\Setup\BootStrapped\4DO_x86.SED.tmp" "4DO_x86.EXE" "4DO_$($releaseVersion)_Setup_x86.exe";
Replace-InFile "$projectRoot\Setup\4DO_x64.SED" "$projectRoot\Setup\BootStrapped\4DO_x64.SED.tmp" "4DO_x64.EXE" "4DO_$($releaseVersion)_Setup_x64.exe";

# Create final files off the temporary sed files.
iexpress /N "$projectRoot\Setup\BootStrapped\4DO_x86.SED.tmp";
iexpress /N "$projectRoot\Setup\BootStrapped\4DO_x64.SED.tmp";
