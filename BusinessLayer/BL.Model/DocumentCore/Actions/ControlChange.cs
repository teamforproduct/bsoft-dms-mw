using System;
using System.ComponentModel.DataAnnotations;

namespace BL.Model.DocumentCore.Actions
{
    public class ControlChange : ControlOn
    {
        [Required]
        public int EventId { get; set; }
    }
}
