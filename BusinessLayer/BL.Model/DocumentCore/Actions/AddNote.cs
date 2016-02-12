using BL.Model.Users;

namespace BL.Model.DocumentCore.Actions
{
    public class AddNote : CurrentPosition
    {
        public int DocumentId { get; set; }
        public string Description { get; set; }
     
    }
}
