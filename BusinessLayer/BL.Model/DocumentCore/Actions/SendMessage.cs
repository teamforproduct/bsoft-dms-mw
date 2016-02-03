using System.Collections.Generic;

namespace BL.Model.DocumentCore.Actions
{
    public class SendMessage
    {
        public int Id { get; set; }
        public List<int> Positions { get; set; }
        public string Description { get; set; }
        public bool IsAddPositionsInfo { get; set; }
    }
}
