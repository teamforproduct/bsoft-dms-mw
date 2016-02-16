using BL.Model.Users;

namespace BL.Model.DocumentCore.Actions
{
    public class ControlOff : CurrentPosition
    {
        public int DocumentId { get; set; }
        public int ResultTypeId { get; set; }
        public string Description { get; set; }
        public bool IsCascade { get; set; }
    }
}
