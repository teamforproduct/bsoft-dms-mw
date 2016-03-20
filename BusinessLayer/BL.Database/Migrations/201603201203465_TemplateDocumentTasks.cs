namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TemplateDocumentTasks : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TemplateDocumentTasks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DocumentId = c.Int(nullable: false),
                        PositionId = c.Int(),
                        Task = c.String(),
                        Description = c.String(),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TemplateDocuments", t => t.DocumentId)
                .ForeignKey("dbo.DictionaryPositions", t => t.PositionId)
                .Index(t => t.DocumentId)
                .Index(t => t.PositionId);
            
            AddColumn("dbo.TemplateDocumentSendListsSet", "TaskId", c => c.Int());
            CreateIndex("dbo.TemplateDocumentSendListsSet", "TaskId");
            AddForeignKey("dbo.TemplateDocumentSendListsSet", "TaskId", "dbo.TemplateDocumentTasks", "Id");
            DropColumn("dbo.TemplateDocumentSendListsSet", "TaskName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TemplateDocumentSendListsSet", "TaskName", c => c.String());
            DropForeignKey("dbo.TemplateDocumentSendListsSet", "TaskId", "dbo.TemplateDocumentTasks");
            DropForeignKey("dbo.TemplateDocumentTasks", "PositionId", "dbo.DictionaryPositions");
            DropForeignKey("dbo.TemplateDocumentTasks", "DocumentId", "dbo.TemplateDocuments");
            DropIndex("dbo.TemplateDocumentTasks", new[] { "PositionId" });
            DropIndex("dbo.TemplateDocumentTasks", new[] { "DocumentId" });
            DropIndex("dbo.TemplateDocumentSendListsSet", new[] { "TaskId" });
            DropColumn("dbo.TemplateDocumentSendListsSet", "TaskId");
            DropTable("dbo.TemplateDocumentTasks");
        }
    }
}
