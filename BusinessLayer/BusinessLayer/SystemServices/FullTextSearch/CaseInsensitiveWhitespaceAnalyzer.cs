using Lucene.Net.Analysis;
using System.IO;
using Lucene.Net.Analysis.Core;
using Version = Lucene.Net.Util.LuceneVersion;

namespace BL.Logic.SystemServices.FullTextSearch
{
    public class CaseInsensitiveWhitespaceAnalyzer : Analyzer
    {

        protected override TokenStreamComponents CreateComponents(string fieldName, TextReader reader)
        {
            Tokenizer t = new WhitespaceTokenizer(Version.LUCENE_48, reader);
            TokenStream s = new LowerCaseFilter(Version.LUCENE_48, t);
            return new TokenStreamComponents(t, s);
        }
    }
}
