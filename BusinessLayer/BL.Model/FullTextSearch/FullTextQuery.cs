using System.Linq;

namespace BL.Model.FullTextSearch
{
    public class FullTextQueryPrepare
    {
        public IQueryable<FullTextIndexItem> Query { get; set; }
        public EnamFilterType FilterType { get; set; }
    }
}
