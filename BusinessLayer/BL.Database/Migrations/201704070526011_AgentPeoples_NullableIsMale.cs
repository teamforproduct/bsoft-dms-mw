namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AgentPeoples_NullableIsMale : DbMigration
    {
        public override void Up()
        {
            AlterColumn("DMS.DictionaryAgentPeoples", "IsMale", c => c.Boolean());
        }
        
        public override void Down()
        {
            AlterColumn("DMS.DictionaryAgentPeoples", "IsMale", c => c.Boolean(nullable: false));
        }
    }
}
