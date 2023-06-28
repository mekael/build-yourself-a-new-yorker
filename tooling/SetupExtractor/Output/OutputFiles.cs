using SetupExtractor.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SetupExtractor.Output
{
    public class OutputFiles
    {
        public string InputFilePath { get; set; }
        public List<FileFromSetup> FilesFromSetup { get; set; }
        public string OutputFolderPath { get; set; }

    }
}
