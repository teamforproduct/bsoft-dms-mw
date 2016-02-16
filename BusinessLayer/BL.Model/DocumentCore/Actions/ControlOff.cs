using BL.Model.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.DocumentCore.Actions
{
    public class ControlOff : CurrentPosition
    {
        [Required]
        public int EventId { get; set; }
        public int DocumentId { get; set; }
        public int ResultTypeId { get; set; }
        public string Description { get; set; }
        public bool IsCascade { get; set; }
    }
}
