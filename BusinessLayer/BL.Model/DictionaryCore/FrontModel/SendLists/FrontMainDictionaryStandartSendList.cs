using BL.Model.Common;
using BL.Model.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.DictionaryCore.FrontModel
{
    public class FrontMainDictionaryStandartSendList : ListItem
    {

        /// <summary>
        /// Код подразделения
        /// </summary>
        public int? ExecutorId { get; set; }

        /// <summary>
        /// Код подразделения
        /// </summary>
        public string ExecutorName { get; set; }


        /// <summary>
        /// Код подразделения
        /// </summary>
        public string ExecutorTypeSuffix { get; set; }

        public virtual IEnumerable<FrontDictionaryStandartSendList> SendLists { get; set; }
    }
}
