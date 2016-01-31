namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Code2 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.DictionaryStandartSendListContents", "TargetAgentId", "dbo.DictionaryAgents");
            DropForeignKey("dbo.DocumentSendLists", "TargetAgentId", "dbo.DictionaryAgents");
            DropForeignKey("dbo.DocumentRestrictedSendLists", "AgentId", "dbo.DictionaryAgents");
            DropIndex("dbo.DictionaryStandartSendListContents", new[] { "TargetAgentId" });
            DropIndex("dbo.DocumentSendLists", new[] { "TargetAgentId" });
            DropIndex("dbo.DocumentRestrictedSendLists", new[] { "AgentId" });
            //DropColumn("dbo.DictionaryStandartSendListContents", "TargetAgentId");
            //DropColumn("dbo.DocumentSendLists", "TargetAgentId");
            //DropColumn("dbo.DocumentRestrictedSendLists", "AgentId");
        }
        
        public override void Down()
        {
            //AddColumn("dbo.DocumentRestrictedSendLists", "AgentId", c => c.Int());
            //AddColumn("dbo.DocumentSendLists", "TargetAgentId", c => c.Int());
            //AddColumn("dbo.DictionaryStandartSendListContents", "TargetAgentId", c => c.Int());
            CreateIndex("dbo.DocumentRestrictedSendLists", "AgentId");
            CreateIndex("dbo.DocumentSendLists", "TargetAgentId");
            CreateIndex("dbo.DictionaryStandartSendListContents", "TargetAgentId");
            AddForeignKey("dbo.DocumentRestrictedSendLists", "AgentId", "dbo.DictionaryAgents", "Id");
            AddForeignKey("dbo.DocumentSendLists", "TargetAgentId", "dbo.DictionaryAgents", "Id");
            AddForeignKey("dbo.DictionaryStandartSendListContents", "TargetAgentId", "dbo.DictionaryAgents", "Id");
        }
    }
}
