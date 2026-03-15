param(
    [string]$Configuration = "Debug"
)

dotnet build .\Wonga.Platform.slnx -c $Configuration

