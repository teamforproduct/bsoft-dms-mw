using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using BL.Model.Common;
using System.ComponentModel.DataAnnotations;

namespace BL.Model.DictionaryCore.IncomingModel
{
    public class AddStandartSendList
    {
        [Required]
        public string Name { get; set; }
        public int? PositionId { get; set; }
        
    }
}
