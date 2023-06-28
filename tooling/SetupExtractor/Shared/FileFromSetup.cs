using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SetupExtractor.Shared
{
    public class FileFromSetup
    {
        public int FileNumber { get; set; }
        public string SourceFilePath { get; set; }
        public string BaseFileName { get; set; }
        public string OriginatingDirectory { get; set; }
        public string FileExtension { get; set; }
        public string RunTimeFolder { get; set; }
        public string FileDescription { get; set; }
        public uint DecompressedSize { get; set; }
        public int AttributesOfOriginalFile { get; set; }
        public string OutputFolder { get; set; }


        public string CustomShortcutLocation { get; set; }
        public string ShortcutComment { get; set; }
        public string ShortcutDescription { get; set; }
        public string ShortcutStartupArgs { get; set; }
        public string ShortcutStartDictionary { get; set; }
        public string IconPath { get; set; }
        public string FontRegName { get; set; }

        public int IsCompressed { get; set; }
        public int OriginalAttributes { get; set; }
        public int ForcedAttributes { get; set; }
        public ushort SkipVal { get; set; }


        public string ScriptCondition { get; set; }
        public string InstallType { get; set; }

        public string FileNotes { get; set; }
        public uint CompressedSize { get; set; }
        public int StartOfFileOffset { get; set; }
        public uint CrcSize { get; set; }

        public int OffsetToStartOfFile { get; set; }

        public bool IsXord { get; set; }
        public string FileName { get; set; }

        public UInt32 CompressedFileSize { get; set; }
        public byte[] CompressedFile { get; set; }
        public byte[] DecompressedFile { get; set; }

        public byte[] Crc { get; set; }

    }
}
