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
    
    public partial class AdminSubordinations
    {
        public int Id { get; set; }
        public int SourcePositionId { get; set; }
        public int TargetPositionId { get; set; }
        public int SubordinationTypeId { get; set; }
        public int LastChangeUserId { get; set; }
        public System.DateTime LastChangeDate { get; set; }
    
        public virtual DictionaryPositions Position { get; set; }
        public virtual DictionaryPositions AddresseePosition { get; set; }
        public virtual DictionarySubordinationTypes SubordinationType { get; set; }
    }
}