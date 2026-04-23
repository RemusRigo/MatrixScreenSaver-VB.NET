dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true
cd "bin\Release\win-x64\publish"
ren MatrixSS.exe MatrixSS.scr