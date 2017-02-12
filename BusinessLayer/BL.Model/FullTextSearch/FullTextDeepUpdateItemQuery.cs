using BL.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.FullTextSearch
{
    public class FullTextDeepUpdateItemQuery
    {
        public EnumObjects ObjectType { get; set; }
        public EnamFilterType FilterType { get; set; }
    }
}
