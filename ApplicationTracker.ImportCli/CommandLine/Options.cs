using CommandLine;

namespace ApplicationTracker.ImportCli.CommandLine
{
    public class Options
    {
        [Option('f', "file", Required = true, HelpText = "Path to the file to open")]
        public string FilePath { get; set; } = string.Empty;

        [Option('r', "report", Required = false, HelpText = "Report entity status")]
        public bool Report { get; set; }

        [Option('x', "execute", Required = false, HelpText = "Execute selected migrations")]
        public bool Execute { get; set; }

        public bool ValidateRequired()
        {
            if (string.IsNullOrWhiteSpace(FilePath))
            {
                Console.WriteLine("Error: The required option '--file (-f)' is missing.");
                return false;
            }
            return true;
        }

        public bool ValidateFilePath()
        {
            if (string.IsNullOrWhiteSpace(FilePath))
            {
                Console.WriteLine("Error: File path is empty.");
                return false;
            }
            if (!File.Exists(FilePath))
            {
                Console.WriteLine($"Error: File not found at '{FilePath}'.");
                return false;
            }
            return true;
        }
    }
}
