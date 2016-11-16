namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IsSendEMail : DbMigration
    {
        public override void Up()
        {
            AddColumn("DMS.DictionaryAgentUsers", "IsSendEMail", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("DMS.DictionaryAgentUsers", "IsSendEMail");
        }
    }
}
