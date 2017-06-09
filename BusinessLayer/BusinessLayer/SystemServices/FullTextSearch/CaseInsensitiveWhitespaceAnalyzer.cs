using Lucene.Net.Analysis;
using System.IO;
using Lucene.Net.Analysis.Core;
using Lucene.Net.Util;

namespace BL.Logic.SystemServices.FullTextSearch
{
    public class CaseInsensitiveWhitespaceAnalyzer : Analyzer
    {
        private readonly LuceneVersion matchVersion;

        public CaseInsensitiveWhitespaceAnalyzer(LuceneVersion matchVersion)
        {
            this.matchVersion = matchVersion;
        }

        protected override TokenStreamComponents CreateComponents(string fieldName, TextReader reader)
        {
            Tokenizer t = new WhitespaceTokenizer(matchVersion, reader);
            TokenStream s = new LowerCaseFilter(matchVersion, t);
            return new TokenStreamComponents(t, s);
        }
    }
}
