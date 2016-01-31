namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Code3 : DbMigration
    {
        public override void Up()
        {
            //DropColumn("dbo.DictionaryStandartSendListContents", "TargetAgentId");
            DropColumn("dbo.DocumentSendLists", "TargetAgentId");
            DropColumn("dbo.DocumentRestrictedSendLists", "AgentId");
        }

        public override void Down()
        {
            //AddColumn("dbo.DocumentRestrictedSendLists", "AgentId", c => c.Int());
            AddColumn("dbo.DocumentSendLists", "TargetAgentId", c => c.Int());
            AddColumn("dbo.DictionaryStandartSendListContents", "TargetAgentId", c => c.Int());
        }
    }
}
