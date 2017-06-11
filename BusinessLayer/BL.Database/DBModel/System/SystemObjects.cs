using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BL.Database.DBModel.System
{

    public  class SystemObjects
    {
        public SystemObjects()
        {
            this.Fields = new HashSet<SystemFields>();
            this.Actions = new HashSet<SystemActions>();
            //this.CalculatedValues = new HashSet<SystemCalculatedValues>();
        }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [MaxLength(400)]
        public string Code { get; set; }   

        public virtual ICollection<SystemFields> Fields { get; set; }

        public virtual ICollection<SystemActions> Actions { get; set; }

        //public virtual ICollection<SystemCalculatedValues> CalculatedValues { get; set; }
    }
}
