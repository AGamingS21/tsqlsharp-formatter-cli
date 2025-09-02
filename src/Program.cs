
using System.CommandLine;
using TSqlSharp.Formating;

namespace tsqlsharpcli
{

    public static class Program
    {
        public static int Main(string[] args)
        {

            var wrkingdir = Directory.GetCurrentDirectory();
            var rootCommand = new RootCommand();

            rootCommand.Description = "tsqlsharp formatter is a tool to format tsql files.";

            var pathOption = new Option<string>("--path", "The path to sql file.");

            pathOption.AddAlias("-p");


            var configureSubCommand = new Command("format", "format the file passed to the cli.");
            // Handles what happens when configure is called
            configureSubCommand.AddOption(pathOption);
            configureSubCommand.SetHandler((path) =>
            {
                if (File.Exists(path))
                {
                    Format(path);
                }
                else
                {
                    Console.WriteLine($"{path} could not be found. please make sure it exists.");
                }

            },
            pathOption);

            rootCommand.Add(configureSubCommand);

            return rootCommand.Invoke(args);
        }

        static void Format(string path)
        {
            var formatter = new Formatter();

            var output = formatter.Format(path);
            
        }

        
    }
    
}


