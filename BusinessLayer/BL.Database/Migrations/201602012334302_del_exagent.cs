using System;
using System.Data.Entity.Migrations;

namespace BL.Database.Migrations
{

    public partial class del_exagent : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Documents", "ExecutorAgentId", "dbo.DictionaryAgents");
            DropIndex("dbo.Documents", new[] { "ExecutorAgentId" });
            DropColumn("dbo.Documents", "ExecutorAgentId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Documents", "ExecutorAgentId", c => c.Int(nullable: false));
            CreateIndex("dbo.Documents", "ExecutorAgentId");
            AddForeignKey("dbo.Documents", "ExecutorAgentId", "dbo.DictionaryAgents", "Id");
        }
    }
}
