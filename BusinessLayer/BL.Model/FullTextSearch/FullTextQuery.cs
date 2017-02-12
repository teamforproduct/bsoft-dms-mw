using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.FullTextSearch
{
    public class FullTextQueryPrepare
    {
        public IQueryable<FullTextIndexItem> Query { get; set; }
        public EnamFilterType FilterType { get; set; }
    }
}
