using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Database.DBModel.Dictionary
{

    public partial class DictionaryCompanies
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int LastChangeUserId { get; set; }
        public System.DateTime LastChangeDate { get; set; }
    }

}
