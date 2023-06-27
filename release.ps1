dotnet publish RegFlow -c Release -r win-x64 --no-self-contained
Compress-Archive -LiteralPath RegFlow/bin/Release/win-x64/publish -DestinationPath RegFlow/bin/RegFlow.zip -Force