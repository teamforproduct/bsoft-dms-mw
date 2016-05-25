namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DocumentTaskAccesses : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "DMS.DocumentTaskAccesses",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TaskId = c.Int(nullable: false),
                        PositionId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.DictionaryPositions", t => t.PositionId)
                .ForeignKey("DMS.DocumentTasks", t => t.TaskId)
                .Index(t => new { t.TaskId, t.PositionId }, unique: true, name: "IX_TaskPosition")
                .Index(t => t.PositionId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("DMS.DocumentTaskAccesses", "TaskId", "DMS.DocumentTasks");
            DropForeignKey("DMS.DocumentTaskAccesses", "PositionId", "DMS.DictionaryPositions");
            DropIndex("DMS.DocumentTaskAccesses", new[] { "PositionId" });
            DropIndex("DMS.DocumentTaskAccesses", "IX_TaskPosition");
            DropTable("DMS.DocumentTaskAccesses");
        }
    }
}
