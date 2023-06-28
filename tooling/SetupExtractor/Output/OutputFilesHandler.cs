using CSPKWare;
using SetupExtractor.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SetupExtractor.Output
{
    public class OutputFilesHandler
    {


        public void Handle(OutputFiles outputFiles)
        {
            byte[] fileContents = File.ReadAllBytes(outputFiles.InputFilePath);



            using (var stream = new MemoryStream(fileContents))
            {

                for (int i = 0; i < outputFiles.FilesFromSetup.Count; i++)
                {
                    var file = outputFiles.FilesFromSetup[i];

                    stream.Position = file.OffsetToStartOfFile;
                    string fileOutputFolderPath = Path.Combine(outputFiles.OutputFolderPath, file.OutputFolder.Replace("%AppFolder%", ""));

                    Directory.CreateDirectory(fileOutputFolderPath);
                    string filePath = Path.Combine(fileOutputFolderPath, file.BaseFileName);

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
    }
}
