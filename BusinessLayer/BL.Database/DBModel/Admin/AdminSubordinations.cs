using BL.Database.DBModel.Dictionary;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Database.DBModel.Admin
{
    public class AdminSubordinations
    {
        public int Id { get; set; }
        public int PositionId { get; set; }
        public int AddresseePositionId { get; set; }
        public int SubordinationTypeId { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }

        [ForeignKey("PositionId")]
        public virtual DictionaryPositions Position { get; set; }
        [ForeignKey("AddresseePositionId")]
        public virtual DictionaryPositions AddresseePosition { get; set; }
        [ForeignKey("SubordinationTypeId")]
        public virtual DictionarySubordinationTypes SubordinationType { get; set; }
    }
}
