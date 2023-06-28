using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SetupExtractor
{
    public class BaseFile
    {
        public int FileNumber { get; set; }
        public bool IsXord { get; set; }
        public string FileName { get; set; }

        public bool IsCompressed { get; set; }
        public UInt32 CompressedFileSize { get; set; }
        public byte[] CompressedFile { get; set; }
        public byte[] DecompressedFile { get; set; }

        public byte[] Crc { get; set; }

    }
}
