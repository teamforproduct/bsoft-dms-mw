using BL.Model.Users;
using System.Collections.Generic;

namespace BL.Model.DocumentCore.Actions
{
    public class SendMessage : CurrentPosition
    {
        public int DocumentId { get; set; }
        public List<int> Positions { get; set; }
        public string Description { get; set; }
        public bool IsAddPositionsInfo { get; set; }
    }
}
