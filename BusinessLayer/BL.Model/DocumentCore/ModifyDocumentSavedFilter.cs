using BL.Model.Users;
using System;

namespace BL.Model.DocumentCore
{
    public class ModifyDocumentSavedFilter : CurrentPosition
    {
        public int Id { get; set; }
        public string Icon { get; set; }
        public object Filter { get; set; }
        public bool IsCommon { get; set; }
    }
}
