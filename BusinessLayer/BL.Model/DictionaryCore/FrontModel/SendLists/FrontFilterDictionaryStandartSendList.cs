using BL.Model.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.DictionaryCore.FrontModel
{
    public class FrontFilterDictionaryStandartSendList
    {
        public virtual IEnumerable<FrontPositionExecutorList> Positions { get; set; }

    }
}
