using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace SetupExtractor
{

    public class CommandLineOptions
    {
        [Option('f', "inputFilePath", Required = true)]
        public string InputFilePath { get; set; }

        [Option('o', "outputFolderPath",Required =true)]
        public string OutputFolderPath { get; set; }
    }
}
