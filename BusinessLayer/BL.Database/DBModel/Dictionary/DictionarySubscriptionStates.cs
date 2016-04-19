using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Database.DBModel.Dictionary
{
    public class DictionarySubscriptionStates
    {
        public int Id { get; set; }
        [MaxLength(2000)]
        [Index("IX_Code", 1, IsUnique = true)]
        public string Code { get; set; }
        [MaxLength(2000)]
        [Index("IX_Name", 1, IsUnique = true)]
        public string Name { get; set; }
        public bool IsSuccess { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }
    }
}
