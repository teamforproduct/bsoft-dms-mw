namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddLogAndSetting : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SystemLogs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Message = c.String(),
                        Trace = c.String(),
                        AgentId = c.Int(),
                        LogDate = c.DateTime(nullable: false),
                        ExecutorAgent_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DictionaryAgents", t => t.ExecutorAgent_Id)
                .Index(t => t.ExecutorAgent_Id);
            
            CreateTable(
                "dbo.SystemSettings",
                c => new
                    {
                        Key = c.String(nullable: false, maxLength: 128),
                        Value = c.String(),
                    })
                .PrimaryKey(t => t.Key);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SystemLogs", "ExecutorAgent_Id", "dbo.DictionaryAgents");
            DropIndex("dbo.SystemLogs", new[] { "ExecutorAgent_Id" });
            DropTable("dbo.SystemSettings");
            DropTable("dbo.SystemLogs");
        }
    }
}
