using BL.Model.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.DictionaryCore.FrontModel
{
    public class FrontMainDictionaryStandartSendList
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? PositionId { get; set; }
        public string PositionName { get; set; }
        public string PositionExecutorName { get; set; }
        public string PositionExecutorTypeSuffix { get; set; }
    }
}
