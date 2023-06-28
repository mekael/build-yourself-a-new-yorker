using CSPKWare;
using SetupExtractor.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SetupExtractor.Parsing.Base
{
    public class BaseFileParsingHandler
    {

        public ParseBaseFileResult Handle(ParseBaseFile opts)
        {
            ParseBaseFileResult result = new ParseBaseFileResult() { 
            FileFromSetups = new List<FileFromSetup>()
            };

            byte[] fileContents = File.ReadAllBytes(opts.InputFilePath);
            int initialOffset = 0;

            // indigo rose "data" doesnt actually start until after the last known section 
            // so we need to find out where that starts. 

            using (var stream = new MemoryStream(fileContents))
            {
                var headers = new System.Reflection.PortableExecutable.PEHeaders(stream);
                // get the start of data
                var endSection = headers.SectionHeaders.OrderByDescending(obd => obd.PointerToRawData).First();
                initialOffset = endSection.SizeOfRawData + endSection.PointerToRawData;

                stream.Position = initialOffset;
                byte[] signature = new byte[8];

                // get the file signature
                stream.Read(signature);

                // skip a single byte cause reasons
                stream.ReadByte();

                // get irsetup.exe
                var irSetupFileSizeByte = new byte[4];
                stream.Read(irSetupFileSizeByte);
                var irSetupFileSize = BitConverter.ToUInt32(irSetupFileSizeByte);

                // not doing anything with this for now. 
                var irSetup = new byte[irSetupFileSize];
                stream.Read(irSetup);


                result.FileFromSetups.Add(
                            new FileFromSetup()
                            {
                                FileNumber = 0,
                                DecompressedFile = irSetup,
                                IsCompressed = 0,
                                FileName = "irsetup.exe",
                                IsXord = true,
                                CompressedFileSize = irSetupFileSize
                            }
                            );

                // get teh number of installer files
                byte[] numberOfFilesByteArray = new byte[4];
                stream.Read(numberOfFilesByteArray);
                var numberOfFiles = BitConverter.ToInt32(numberOfFilesByteArray);

                // get the base files from wherever
                for (int i = 1; i <= numberOfFiles; i++)
                {
                    var fileNameByteArray = new byte[260];
                    stream.Read(fileNameByteArray);

                    var fileLengthByteArray = new byte[4];
                    stream.Read(fileLengthByteArray);

                    var crcByteArray = new byte[4];
                    stream.Read(crcByteArray);

                    var fileLength = BitConverter.ToUInt32(fileLengthByteArray);
                    var file = new byte[fileLength];
                    stream.Read(file);

                    result.FileFromSetups.Add(new FileFromSetup()
                    {
                        FileNumber = i,
                        CompressedFile = file,
                        CompressedFileSize = fileLength,
                        DecompressedFile = PKWare.Explode(file),
                        IsCompressed = 0,
                        FileName = System.Text.Encoding.Default.GetString(fileNameByteArray).Replace('\0', ' ').Trim(),
                        IsXord = false,
                        CrcSize= BitConverter.ToUInt32(crcByteArray)
                });
                }

                result.StartOfTNYFiles = stream.Position;
            }

            return result;

        }
    }
}
