using SetupExtractor.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SetupExtractor.Parsing.Base
{
    public class ParseBaseFileResult
    {
        public List<FileFromSetup> FileFromSetups { get; set; }
        public long StartOfTNYFiles { get; set; }

    }
}
