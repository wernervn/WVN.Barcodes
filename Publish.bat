@echo OFF
set nugetversion=1.0.0
set packageid=WVN.Barcodes
set packagepath=./artifacts/%packageid%.%nugetversion%.nupkg
set src=.\src\%packageid%.csproj

dotnet build %src% -c Release
dotnet pack %src% -c Release -o ./artifacts /p:Version=%nugetversion%

rem test nuget
rem nuget push %packagepath% -Source "C:\Box\NuGet"

rem WVN Nuget
rem nuget delete %packageid% %nugetversion% -Source https://api.nuget.org/v3/index.json -ApiKey %WVN_NUGET_API_KEY% -NonInteractive
rem nuget push %packagepath% %WVN_NUGET_API_KEY% -Source https://api.nuget.org/v3/index.json

rem WVN github
dotnet nuget push %packagepath%  --source "github"