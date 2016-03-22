using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Database.DBModel.System
{

    public class SystemValueTypes
    {
        public int Id { get; set; }
        [MaxLength(2000)]
        public string Code { get; set; }
        [MaxLength(2000)]
        public string Description { get; set; }
    }
}
