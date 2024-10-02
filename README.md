A quick PoC for a dotnet tool that could be used to verify whether a nuget package targets the expected TFMs post build.

Rough usage example:

```bash
dotnet verify-package --tfms "net9.0-ios17.5" --package-path "/bin/Release/CommunityToolkit.Maui.1.0.0-pre1.nupkg.zip"
```
