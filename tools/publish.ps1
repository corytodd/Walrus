# Windows x64 build
dotnet publish Walrus.CLI -o dist/win10-x64 --self-contained -c Release -r win10-x64 -p:PublishSingleFile=true