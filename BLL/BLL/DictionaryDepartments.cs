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
    
    public partial class DictionaryDepartments
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DictionaryDepartments()
        {
            this.ChildDepartments = new HashSet<DictionaryDepartments>();
        }
    
        public int Id { get; set; }
        public Nullable<int> ParentId { get; set; }
        public int CompanyId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }
        public int ChiefPositionId { get; set; }
        public bool IsActive { get; set; }
        public int LastChangeUserId { get; set; }
        public System.DateTime LastChangeDate { get; set; }
    
        public virtual DictionaryDepartments ParentDepartment { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DictionaryDepartments> ChildDepartments { get; set; }
        public virtual DictionaryCompanies Company { get; set; }
        public virtual DictionaryPositions ChiefPosition { get; set; }
    }
}