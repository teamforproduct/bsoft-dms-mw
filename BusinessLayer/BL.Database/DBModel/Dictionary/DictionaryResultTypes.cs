using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Database.DBModel.Dictionary
{
    public class DictionaryResultTypes
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsExecute { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }
    }
}
