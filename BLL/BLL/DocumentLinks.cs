//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BLL.BLL
{
    using System;
    using System.Collections.Generic;
    
    public partial class DocumentLinks
    {
        public int Id { get; set; }
        public int DocumentId { get; set; }
        public int ParentDocumentId { get; set; }
        public int LinkTypeId { get; set; }
        public int LastChangeUserId { get; set; }
        public System.DateTime LastChangeDate { get; set; }
    
        public virtual Documents Document { get; set; }
        public virtual Documents ParentDocument { get; set; }
        public virtual DictionaryLinkTypes LinkType { get; set; }
    }
}