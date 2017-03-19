using Lucene.Net.Analysis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Logic.SystemServices.FullTextSearch
{
    public class CaseInsensitiveWhitespaceAnalyzer : Analyzer
    {
        /// <summary>
        /// </summary>
        public override TokenStream TokenStream(string fieldName, TextReader reader)
        {
            TokenStream t = null;
            t = new WhitespaceTokenizer(reader);
            t = new LowerCaseFilter(t);

            return t;
        }
    }
}
