namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserName : DbMigration
    {
        public override void Up()
        {
            AddColumn("DMS.DictionaryAgentUsers", "UserName", c => c.String(maxLength: 256));
            DropColumn("DMS.DictionaryAgentUsers", "IsActive");
        }
        
        public override void Down()
        {
            AddColumn("DMS.DictionaryAgentUsers", "IsActive", c => c.Boolean(nullable: false));
            DropColumn("DMS.DictionaryAgentUsers", "UserName");
        }
    }
}
