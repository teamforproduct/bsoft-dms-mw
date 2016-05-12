namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class WaitDescription : DbMigration
    {
        public override void Up()
        {
            AddColumn("DMS.DictionaryEventTypes", "WaitDescription", c => c.String(maxLength: 2000));
        }
        
        public override void Down()
        {
            DropColumn("DMS.DictionaryEventTypes", "WaitDescription");
        }
    }
}
