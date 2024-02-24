using Spectre.Console;
using Spectre.Console.Cli;

namespace az
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var app = new CommandApp<DirectoryListCommand>();
            app.Configure(config =>
            {
                config.SetApplicationName("az");
                config.SetApplicationVersion("1.0.0");
            });

            app.Run(args);
        }
    }

    public class DirectoryListCommand : Command<Settings>
    {
        private static readonly BarChart DirectoryInfo = new();

        private static int _directoryCount;
        private static int _fileCount;
        public override int Execute(CommandContext context, Settings settings)
        {
            // Initialize the main routine.
            ListDirectoryContents(new DirectoryInfo(settings.Path), settings.Recursive, settings.ShowHidden);

            // Configure the bar chart.
            DirectoryInfo.Label = "Directory Contents";
            DirectoryInfo.Width = 50;
            DirectoryInfo.AddItem("Directories", _directoryCount, Color.Green);
            DirectoryInfo.AddItem("Files", _fileCount, Color.Red);

            // Display the bar chart.
            AnsiConsole.Write(DirectoryInfo);
            
            // Return the exit code.
            return 0;
        }

        private static void ListDirectoryContents(DirectoryInfo directory, bool recursive, bool hidden)
        {
            // Increment the directory and file count.
            _directoryCount += directory.GetDirectories().Length;
            _fileCount += directory.GetFiles().Length;

            // Configure the tree.
            Tree root = new($"{directory.Name}");
            root.Guide(TreeGuide.Line);
            root.Style(Style.Parse("green"));

            // Configure and go through the items.
            var items = directory.GetFileSystemInfos();
            if (!hidden) items = Array.FindAll(items, f => !f.Attributes.HasFlag(FileAttributes.Hidden));
            foreach (var item in items)
            {
                if (item is DirectoryInfo info)
                {
                    root.AddNode(info.Name);
                    if (recursive) ListDirectoryContents(info, true, hidden);
                }
                else
                {
                    root.AddNode(item.Name);
                }
            }

            // Display the tree.
            AnsiConsole.Write(root);
        }
    }
}
