using Lucene.Net.Analysis;
using System;
using System.IO;
using Lucene.Net.Analysis.Core;
using Version = Lucene.Net.Util.LuceneVersion;

namespace BL.Logic.SystemServices.FullTextSearch
{
    public class CaseInsensitiveWhitespaceAnalyzer : Analyzer
    {
        /// <summary>
        /// </summary>
        public new TokenStream GetTokenStream(string fieldName, TextReader reader)
        {
            TokenStream t = new WhitespaceTokenizer(Version.LUCENE_48, reader);
            t = new LowerCaseFilter(Version.LUCENE_48, t);

            return t;
        }

        protected override TokenStreamComponents CreateComponents(string fieldName, TextReader reader)
        {
            throw new NotImplementedException();
        }
    }
}
