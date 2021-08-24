# https://docs.microsoft.com/en-us/dotnet/core/tools/global-tools-how-to-use
rm ./dist/*.nupkg
dotnet pack Walrus.CLI
dotnet tool uninstall --global corytodd.us.walrusc
dotnet tool install --global --add-source ./dist corytodd.us.walrusc