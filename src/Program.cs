
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

            var outputPathOption = new Option<string>("--output", "The output path for the file sql file.");
            outputPathOption.AddAlias("-o");


            var configureSubCommand = new Command("format", "format the file passed to the cli.");
            // Handles what happens when configure is called
            configureSubCommand.AddOption(pathOption);
            configureSubCommand.AddOption(outputPathOption);
            configureSubCommand.SetHandler((path, output) =>
            {
                if (File.Exists(path))
                {
                    Format(path, output);
                }
                else
                {
                    Console.WriteLine($"{path} could not be found. please make sure it exists.");
                }

            },
            pathOption, outputPathOption);

            rootCommand.Add(configureSubCommand);

            return rootCommand.Invoke(args);
        }

        static void Format(string path, string outputPath)
        {
            if (outputPath == "")
            {
                outputPath = path;
            }
            var formatter = new Formatter();

            var output = formatter.Format(path);
            
            using (StreamWriter outputFile = new StreamWriter(outputPath, false))
            {
                outputFile.WriteLine(output);
            }   
        }

        
    }
    
}


