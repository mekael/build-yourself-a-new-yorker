using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SetupExtractor.Shared;

namespace SetupExtractor.Parsing
{
    public class IrSetupFileParsingHandler
    {


        public List<FileFromSetup> Handle(byte[] file, long initialOffsetFromStartOfExe)
        {
            byte[] sectionHeader = System.Text.Encoding.Default.GetBytes("CSetupFileData");

            // find the start of our data section
            var startOfDataSection = file.AsSpan().IndexOf(sectionHeader) - 8;
            List<FileFromSetup> filesFromSetup = new List<FileFromSetup>();

            // get the framework install exe length and add to our offset

            UInt32 dotnetOffset = GetDotnetFrameworkInfo(file);


            int currentOffset = initialOffsetFromStartOfExe + (int)dotnetOffset;



            using (var stream = new MemoryStream(file))
            {
                stream.Position = startOfDataSection;

                // get the number of file definitions in the section (first 2 bytes)
                byte[] numItems = new byte[2];

                stream.Read(numItems);
                UInt16 numberOfItems = BitConverter.ToUInt16(numItems);

                // Skip 2 unknown uint16_t numbers, always 0xFFFF and 0x0001
                stream.Seek(4, SeekOrigin.Current);

                // skip sectionHeader
                var whatever = GetString(stream);

                // for some reason there is a single "offset" byte here
                stream.Seek(1, SeekOrigin.Current);

                // skip 5 bytes
                stream.Seek(5, SeekOrigin.Current);


                List<string> fileNames = new List<string>();


               

                for (int i = 0; i < numberOfItems; i++)
                {
                    FileFromSetup fileFromSetup = new FileFromSetup();

                    fileFromSetup.OffsetToStartOfFile = currentOffset;

                    fileFromSetup.FileNumber = i;
                    fileFromSetup.SourceFilePath = GetString(stream);
                    fileFromSetup.BaseFileName = GetString(stream);
                    fileFromSetup.OriginatingDirectory = GetString(stream);
                    fileFromSetup.FileExtension = GetString(stream);
                    fileFromSetup.RunTimeFolder = GetString(stream);
                    fileFromSetup.FileDescription = GetString(stream);

                    stream.Seek(2, SeekOrigin.Current);

                    byte[] decompSizeArray = new byte[4];
                    stream.Read(decompSizeArray);
                    fileFromSetup.DecompressedSize = BitConverter.ToUInt32(decompSizeArray);

                    fileFromSetup.AttributesOfOriginalFile = stream.ReadByte();

                    stream.Seek(37, SeekOrigin.Current);
                    fileFromSetup.OutputFolder = GetString(stream);

                    // skip 10 
                    stream.Seek(10, SeekOrigin.Current);
                    fileFromSetup.CustomShortcutLocation = GetString(stream);
                    fileFromSetup.ShortcutComment = GetString(stream);
                    fileFromSetup.ShortcutDescription = GetString(stream);
                    fileFromSetup.ShortcutStartupArgs = GetString(stream);
                    fileFromSetup.ShortcutStartDictionary = GetString(stream);


                    // skip 1 byte
                    stream.Seek(1, SeekOrigin.Current);

                    fileFromSetup.IconPath = GetString(stream);
                    // skip 8 bytes
                    stream.Seek(8, SeekOrigin.Current);

                    fileFromSetup.FontRegName = GetString(stream);

                    // skip 3 bytes
                    stream.Seek(3, SeekOrigin.Current);

                    fileFromSetup.IsCompressed = stream.ReadByte();
                    fileFromSetup.OriginalAttributes = stream.ReadByte();
                    fileFromSetup.ForcedAttributes = stream.ReadByte();

                    // skip 10 
                    stream.Seek(10, SeekOrigin.Current);

                    // skipVal (uint16)
                    byte[] skipValArray = new byte[2];
                    stream.Read(skipValArray);
                    fileFromSetup.SkipVal = BitConverter.ToUInt16(skipValArray);

                    // skip (skipval*2)
                    stream.Seek(fileFromSetup.SkipVal * 2, SeekOrigin.Current);


                    fileFromSetup.ScriptCondition = GetString(stream);

                    // skip 2 bytes
                    stream.Seek(2, SeekOrigin.Current);

                    // install type
                    fileFromSetup.InstallType = GetString(stream);



                    // something to be skipped (unkown string) 
                    string unkownString = GetString(stream);


                    // uint16 packageNum
                    byte[] packageNumArray = new byte[2];
                    stream.Read(packageNumArray);
                    UInt16 packageNum = BitConverter.ToUInt16(packageNumArray);
                    // skip the # of package nums  (which contain package names
                    for (int j = 0; i < packageNum; i++)
                    {
                        GetString(stream); // Package name
                    }

                    fileFromSetup.FileNotes = GetString(stream);

                    // uint32 compression size
                    byte[] compressionSizeArray = new byte[4];
                    stream.Read(compressionSizeArray);
                    fileFromSetup.CompressedSize = BitConverter.ToUInt32(compressionSizeArray);

                    // uint32 crc size
                    byte[] crcSizeArray = new byte[4];
                    stream.Read(crcSizeArray);
                    fileFromSetup.CrcSize = BitConverter.ToUInt32(crcSizeArray);

                    // skip 8 bytes
                    stream.Seek(8, SeekOrigin.Current);
                    filesFromSetup.Add(fileFromSetup);

                    currentOffset += (int)fileFromSetup.CompressedSize;

                }

            }
            return filesFromSetup;
        }


        UInt32 GetDotnetFrameworkInfo(byte[] file)
        {
            UInt32 dotnetOffset = 0;
            byte[] sectionHeader = System.Text.Encoding.Default.GetBytes("CDependencyFile");

            // find the start of our data section
            var startOfDataSection = file.AsSpan().IndexOf(sectionHeader) - 8;

            using (var stream = new MemoryStream(file))
            {
                stream.Position = startOfDataSection;

                // get the number of file definitions in the section (first 2 bytes)
                byte[] numItems = new byte[2];

                stream.Read(numItems);
                UInt16 numberOfItems = BitConverter.ToUInt16(numItems);

                // Skip 2 unknown uint16_t numbers, always 0xFFFF and 0x0001
                stream.Seek(4, SeekOrigin.Current);

                // skip sectionHeader
                var whatever = GetString(stream);

                // for some reason there is a single "offset" byte  
                stream.Seek(1, SeekOrigin.Current);
                // skip 3 bytes
                stream.Seek(4, SeekOrigin.Current);
                var originatingFilePath = GetString(stream);

                byte[] compressionSizeArray = new byte[4];
                stream.Read(compressionSizeArray);
                dotnetOffset = BitConverter.ToUInt32(compressionSizeArray);
            }
            return dotnetOffset;

        }


        string GetString(MemoryStream memoryStream)
        {
            string returnValue = "";

            int lengthToRead = memoryStream.ReadByte();

            if (lengthToRead > 0)
            {
                byte[] stringByte = new byte[lengthToRead];
                memoryStream.Read(stringByte);
                returnValue = Encoding.UTF8.GetString(stringByte);

            }
            return returnValue;
        }
    }

    
}
