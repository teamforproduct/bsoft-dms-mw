using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.Model.Common;
using BL.Model.Enums;
using BL.Model.SystemCore.IncomingModel;
using System.ComponentModel.DataAnnotations;

namespace BL.Model.DocumentCore.IncomingModel
{
    public class ModifyTemplateDocument : AddTemplateDocument
    {
        [Required]
        public int Id { get; set; }
    }
}
