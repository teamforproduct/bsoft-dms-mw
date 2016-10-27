using System.Collections.Generic;

namespace BL.Model.SystemCore.FrontModel
{
    public class FrontSystemObject
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }

        public IEnumerable<FrontSystemAction> Actions { get; set; }
    }
}