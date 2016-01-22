@echo off
rem -------------------------------------------------
rem SeriesRenamer build script (RELEASE)
rem -------------------------------------------------
SET build="C:\Program Files (x86)\MSBuild\14.0\Bin\msbuild.exe"
SET root=%~dp0\..

echo -------------------------------------------------
echo Restoring .nuget Packages

FOR /R "..\" %%G IN (*.sln) DO NuGet.exe restore "%%G"

echo -------------------------------------------------
echo Build...
%build% %root%\Renamer.sln /p:Configuration=Release /verbosity:minimal /p:DebugSymbols=false /p:DebugType=None /p:OutputPath=%root%\Publish