// See https://aka.ms/new-console-template for more i
using SetupExtractor.Parsing;
using CSPKWare;
using CommandLine;
using SetupExtractor.Shared;
using SetupExtractor.Parsing.Base;

namespace SetupExtractor
{
    public static class Program
    {

        public static void Main(string[] args)
        {

            CommandLine.Parser.Default.ParseArguments<CommandLineOptions>(args)
                                  .WithParsed(RunOptions)
                                  .WithNotParsed(HandleParseError);


        }


        static void RunOptions(CommandLineOptions opts)
        {

            
            ParseBaseFileResult parseBaseFileResult = new BaseFileParsingHandler().Handle(new ParseBaseFile() { InputFilePath = opts.InputFilePath }); 
            
            var filesFromSetup = new IrSetupFileParsingHandler().Handle(parseBaseFileResult.FileFromSetups.First(f => f.FileName.Equals("irsetup.dat")).DecompressedFile, parseBaseFileResult.StartOfTNYFiles);

            using (var stream = new MemoryStream(File.ReadAllBytes(opts.InputFilePath)))
            {

                for (int i = 0; i < filesFromSetup.Count; i++)
                {
                    var file = filesFromSetup[i];

                    stream.Position = file.OffsetToStartOfFile;
                    string fileOutputFolderPath = Path.Combine(opts.OutputFolderPath,file.OutputFolder.Replace("%AppFolder%", ""));

                    Directory.CreateDirectory(fileOutputFolderPath);
                    string filePath = Path.Combine( fileOutputFolderPath, file.BaseFileName);

                    byte[] fileContentsArray;

                    if (file.IsCompressed == 0)
                    {
                        fileContentsArray = new byte[file.DecompressedSize];
                        stream.Read(fileContentsArray);
                        File.WriteAllBytes(filePath, fileContentsArray);
                    }
                    else
                    {
                        var compressedFile = new byte[file.CompressedSize];
                        stream.Read(compressedFile);
                        fileContentsArray = PKWare.Explode(compressedFile);
                    }
                    File.WriteAllBytes(filePath, fileContentsArray);
                }
            }

        }
        static void HandleParseError(IEnumerable<Error> errs)
        {
            //handle errors
        }

 


    }



}