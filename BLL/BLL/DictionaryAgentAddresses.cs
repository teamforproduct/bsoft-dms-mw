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
    
    public partial class DictionaryAgentAddresses
    {
        public int Id { get; set; }
        public int AgentId { get; set; }
        public int AdressTypeId { get; set; }
        public string PostCode { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public int LastChangeUserId { get; set; }
        public System.DateTime LastChangeDate { get; set; }
    
        public virtual DictionaryAgents Agent { get; set; }
        public virtual DictionaryAddressTypes AddressType { get; set; }
    }
}