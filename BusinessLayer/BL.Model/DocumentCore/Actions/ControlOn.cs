using BL.Model.Users;
using System;

namespace BL.Model.DocumentCore.Actions
{
    public class ControlOn : CurrentPosition
    {
        public int DocumentId { get; set; }
        public string Description { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? AttentionDate { get; set; }
    }
}
