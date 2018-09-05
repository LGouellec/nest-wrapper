del nupkgs\* /F /Q
dotnet restore
dotnet build
dotnet pack --no-build --output nupkgs
dotnet nuget push nupkgs\*.nupkg --api-key oy2k54q4szxb3att3huqg276tbseb4cu5ibsk4t5cy7wfq --source https://www.nuget.org/
del nupkgs\* /F /Q