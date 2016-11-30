using BL.Model.Extensions;
using System;

namespace BL.Model.DocumentCore.FrontModel
{
    public class FrontDocumentSavedFilter
    {
        public int Id { get; set; }
        public Nullable<int> UserId { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public object Filter { get; set; }
        public bool IsCommon { get; set; }
        public int LastChangeUserId { get; set; }
		
        private DateTime  _LastChangeDate; 
        public DateTime LastChangeDate { get { return _LastChangeDate; } set { _LastChangeDate=value.ToUTC(); } }
		
        public string UserName { get; set; }
    }
}
