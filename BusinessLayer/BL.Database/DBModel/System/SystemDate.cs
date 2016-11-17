using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BL.Database.DBModel.System
{

    public class SystemDate
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
    }
}
