
using System.CommandLine;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
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

            var configureFormatTextCommand = new Command("text", "format the text passed to the cli.");
            var textOption = new Option<string>("--input", "The raw sql text.");
            textOption.AddAlias("-i");
            configureFormatTextCommand.AddOption(textOption);

            var configureFileSubCommand = new Command("file", "format the file passed to the cli.");
            // Handles what happens when configure is called
            configureFileSubCommand.AddOption(pathOption);
            configureFileSubCommand.AddOption(outputPathOption);
            configureFileSubCommand.SetHandler((path, output) =>
            {
                if (File.Exists(path))
                {
                    FormatFile(path, output);
                }
                else
                {
                    Console.WriteLine($"{path} could not be found. please make sure it exists.");
                }

            },
            pathOption, outputPathOption);

            configureFormatTextCommand.SetHandler((text) =>
            {
                try
                {
                    FormatText(text);
                }
                catch
                {
                    Console.WriteLine("Unknown Error occured.");
                }
            },textOption);

            rootCommand.Add(configureFormatTextCommand);
            rootCommand.Add(configureFileSubCommand);

            return rootCommand.Invoke(args);
        }

        static void FormatFile(string path, string outputPath)
        {
            if (outputPath == null)
            {
                outputPath = path;
            }
            var formatter = new Formatter();
            var output = "";
            using (var streamReader = new StreamReader(path))
            {
                output = formatter.Format(streamReader.ReadToEnd());
            }


            using (StreamWriter outputFile = new StreamWriter(outputPath, false))
            {
                outputFile.WriteLine(output);
            }
        }

        static void FormatText(string input)
        {
            var ouputObj = new OutputObject();
            try
            {
                var formatter = new Formatter();
                ouputObj.Output = formatter.Format(input);
                ouputObj.Succeeded = true;

            }
            catch (Exception ex)
            {
                ouputObj.Output = ex.Message;
                ouputObj.Succeeded = false;
            }

            var options = new JsonSerializerOptions
            {
                TypeInfoResolver = OutputObjectSourceGenerationContext.Default,
                // Converters = { new FooConverter() },
            };
            
            Console.WriteLine(JsonSerializer.Serialize(ouputObj, options));
        }


    }

    // Since this project is running in AOT this will need to be defined to serialize it to json.
    [JsonSerializable(typeof(OutputObject))]
    [JsonSerializable(typeof(bool))]        
    [JsonSerializable(typeof(string))]        
    internal partial class OutputObjectSourceGenerationContext : JsonSerializerContext
    {
    }
    public class OutputObject
    {
        public bool Succeeded { get; set; }
        public string Output { get; set; }
    }
    
}


