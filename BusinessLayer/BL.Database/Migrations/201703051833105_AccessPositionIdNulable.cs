namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AccessPositionIdNulable : DbMigration
    {
        public override void Up()
        {
            DropIndex("DMS.DocumentAccesses", "IX_DocumentPosition");
            DropIndex("DMS.DocumentAccesses", "IX_PositionDocument");
            AddColumn("DMS.DocumentAccesses", "AgentId", c => c.Int());
            AlterColumn("DMS.DocumentAccesses", "PositionId", c => c.Int());
            CreateIndex("DMS.DocumentAccesses", new[] { "DocumentId", "PositionId" }, unique: true, name: "IX_DocumentPosition");
            CreateIndex("DMS.DocumentAccesses", new[] { "PositionId", "DocumentId" }, unique: true, name: "IX_PositionDocument");
            CreateIndex("DMS.DocumentAccesses", "AgentId");
            AddForeignKey("DMS.DocumentAccesses", "AgentId", "DMS.DictionaryAgents", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("DMS.DocumentAccesses", "AgentId", "DMS.DictionaryAgents");
            DropIndex("DMS.DocumentAccesses", new[] { "AgentId" });
            DropIndex("DMS.DocumentAccesses", "IX_PositionDocument");
            DropIndex("DMS.DocumentAccesses", "IX_DocumentPosition");
            AlterColumn("DMS.DocumentAccesses", "PositionId", c => c.Int(nullable: false));
            DropColumn("DMS.DocumentAccesses", "AgentId");
            CreateIndex("DMS.DocumentAccesses", new[] { "PositionId", "DocumentId" }, unique: true, name: "IX_PositionDocument");
            CreateIndex("DMS.DocumentAccesses", new[] { "DocumentId", "PositionId" }, unique: true, name: "IX_DocumentPosition");
        }
    }
}
