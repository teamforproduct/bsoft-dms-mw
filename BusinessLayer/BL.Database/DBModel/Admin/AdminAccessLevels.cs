using System;


namespace BL.Database.DBModel.Admin
{
    public class AdminAccessLevels
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }
    }
}
