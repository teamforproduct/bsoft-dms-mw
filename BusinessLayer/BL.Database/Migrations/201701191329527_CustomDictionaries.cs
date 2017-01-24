namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CustomDictionaries : DbMigration
    {
        public override void Up()
        {
            AddColumn("DMS.CustomDictionaries", "Name", c => c.String(maxLength: 2000));
            AddColumn("DMS.CustomDictionaryTypes", "Name", c => c.String(maxLength: 2000));
        }
        
        public override void Down()
        {
            DropColumn("DMS.CustomDictionaryTypes", "Name");
            DropColumn("DMS.CustomDictionaries", "Name");
        }
    }
}
