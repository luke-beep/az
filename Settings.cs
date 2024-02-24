using Spectre.Console;
using Spectre.Console.Cli;

namespace az;

public sealed class Settings : CommandSettings
{
    [CommandArgument(0,"[path]")]
    public string Path { get; set; } = Directory.GetCurrentDirectory();

    [CommandOption("-r|--recurse")]
    public bool Recursive { get; set; } = false;

    [CommandOption("-h|--hidden")]
    public bool ShowHidden { get; set; } = false;

    public override ValidationResult Validate()
    {
        return !Directory.Exists(Path) ? ValidationResult.Error($"The directory '{Path}' does not exist.") : base.Validate();
    }
}