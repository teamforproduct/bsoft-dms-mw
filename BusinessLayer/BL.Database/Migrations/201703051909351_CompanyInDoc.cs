namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CompanyInDoc : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("DMS.Documents", "DocumentSubjectId", "DMS.DictionaryDocumentSubjects");
            DropForeignKey("DMS.TemplateDocuments", "DocumentSubjectId", "DMS.DictionaryDocumentSubjects");
            DropIndex("DMS.Documents", new[] { "DocumentSubjectId" });
            DropIndex("DMS.TemplateDocuments", new[] { "DocumentSubjectId" });
            CreateTable(
                "DMS.DocumentEventAccesses",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClientId = c.Int(nullable: false),
                        DocumentId = c.Int(nullable: false),
                        EventId = c.Int(nullable: false),
                        PositionId = c.Int(),
                        AgentId = c.Int(),
                        SendDate = c.DateTime(),
                        ReadDate = c.DateTime(),
                        ReadAgentId = c.Int(),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.DictionaryAgents", t => t.AgentId)
                .ForeignKey("DMS.Documents", t => t.DocumentId)
                .ForeignKey("DMS.DocumentEvents", t => t.EventId)
                .ForeignKey("DMS.DictionaryPositions", t => t.PositionId)
                .ForeignKey("DMS.DictionaryAgents", t => t.ReadAgentId)
                .Index(t => t.ClientId)
                .Index(t => t.DocumentId)
                .Index(t => t.EventId)
                .Index(t => t.PositionId)
                .Index(t => t.AgentId)
                .Index(t => t.ReadAgentId);
            
            AddColumn("DMS.DocumentAccesses", "ClientId", c => c.Int(nullable: false));
            AddColumn("DMS.DocumentSubscriptions", "ClientId", c => c.Int(nullable: false));
            AddColumn("DMS.Documents", "ClientId", c => c.Int(nullable: false));
            AddColumn("DMS.Documents", "DocumentDirectionId", c => c.Int());
            AddColumn("DMS.Documents", "DocumentTypeId", c => c.Int());
            AddColumn("DMS.Documents", "DocumentSubject", c => c.String(maxLength: 2000));
            AddColumn("DMS.DocumentEvents", "ClientId", c => c.Int(nullable: false));
            AddColumn("DMS.DocumentSendLists", "ClientId", c => c.Int(nullable: false));
            AddColumn("DMS.DocumentTasks", "ClientId", c => c.Int(nullable: false));
            AddColumn("DMS.DocumentTaskAccesses", "ClientId", c => c.Int(nullable: false));
            AddColumn("DMS.DocumentWaits", "ClientId", c => c.Int(nullable: false));
            AddColumn("DMS.DocumentPapers", "ClientId", c => c.Int(nullable: false));
            AddColumn("DMS.DocumentFiles", "ClientId", c => c.Int(nullable: false));
            AddColumn("DMS.DocumentLinks", "ClientId", c => c.Int(nullable: false));
            AddColumn("DMS.DocumentRestrictedSendLists", "ClientId", c => c.Int(nullable: false));
            AddColumn("DMS.DocumentTags", "ClientId", c => c.Int(nullable: false));
            AddColumn("DMS.TemplateDocuments", "DocumentSubject", c => c.String(maxLength: 2000));
            CreateIndex("DMS.DocumentAccesses", "ClientId");
            CreateIndex("DMS.DocumentSubscriptions", "ClientId");
            CreateIndex("DMS.Documents", "ClientId");
            CreateIndex("DMS.DocumentEvents", "ClientId");
            CreateIndex("DMS.DocumentSendLists", "ClientId");
            CreateIndex("DMS.DocumentTasks", "ClientId");
            CreateIndex("DMS.DocumentTaskAccesses", "ClientId");
            CreateIndex("DMS.DocumentWaits", "ClientId");
            CreateIndex("DMS.DocumentPapers", "ClientId");
            CreateIndex("DMS.DocumentFiles", "ClientId");
            CreateIndex("DMS.DocumentLinks", "ClientId");
            CreateIndex("DMS.DocumentRestrictedSendLists", "ClientId");
            CreateIndex("DMS.DocumentTags", "ClientId");
        }
        
        public override void Down()
        {
            DropForeignKey("DMS.DocumentEventAccesses", "ReadAgentId", "DMS.DictionaryAgents");
            DropForeignKey("DMS.DocumentEventAccesses", "PositionId", "DMS.DictionaryPositions");
            DropForeignKey("DMS.DocumentEventAccesses", "EventId", "DMS.DocumentEvents");
            DropForeignKey("DMS.DocumentEventAccesses", "DocumentId", "DMS.Documents");
            DropForeignKey("DMS.DocumentEventAccesses", "AgentId", "DMS.DictionaryAgents");
            DropIndex("DMS.DocumentEventAccesses", new[] { "ReadAgentId" });
            DropIndex("DMS.DocumentEventAccesses", new[] { "AgentId" });
            DropIndex("DMS.DocumentEventAccesses", new[] { "PositionId" });
            DropIndex("DMS.DocumentEventAccesses", new[] { "EventId" });
            DropIndex("DMS.DocumentEventAccesses", new[] { "DocumentId" });
            DropIndex("DMS.DocumentEventAccesses", new[] { "ClientId" });
            DropIndex("DMS.DocumentTags", new[] { "ClientId" });
            DropIndex("DMS.DocumentRestrictedSendLists", new[] { "ClientId" });
            DropIndex("DMS.DocumentLinks", new[] { "ClientId" });
            DropIndex("DMS.DocumentFiles", new[] { "ClientId" });
            DropIndex("DMS.DocumentPapers", new[] { "ClientId" });
            DropIndex("DMS.DocumentWaits", new[] { "ClientId" });
            DropIndex("DMS.DocumentTaskAccesses", new[] { "ClientId" });
            DropIndex("DMS.DocumentTasks", new[] { "ClientId" });
            DropIndex("DMS.DocumentSendLists", new[] { "ClientId" });
            DropIndex("DMS.DocumentEvents", new[] { "ClientId" });
            DropIndex("DMS.Documents", new[] { "ClientId" });
            DropIndex("DMS.DocumentSubscriptions", new[] { "ClientId" });
            DropIndex("DMS.DocumentAccesses", new[] { "ClientId" });
            DropColumn("DMS.TemplateDocuments", "DocumentSubject");
            DropColumn("DMS.DocumentTags", "ClientId");
            DropColumn("DMS.DocumentRestrictedSendLists", "ClientId");
            DropColumn("DMS.DocumentLinks", "ClientId");
            DropColumn("DMS.DocumentFiles", "ClientId");
            DropColumn("DMS.DocumentPapers", "ClientId");
            DropColumn("DMS.DocumentWaits", "ClientId");
            DropColumn("DMS.DocumentTaskAccesses", "ClientId");
            DropColumn("DMS.DocumentTasks", "ClientId");
            DropColumn("DMS.DocumentSendLists", "ClientId");
            DropColumn("DMS.DocumentEvents", "ClientId");
            DropColumn("DMS.Documents", "DocumentSubject");
            DropColumn("DMS.Documents", "DocumentTypeId");
            DropColumn("DMS.Documents", "DocumentDirectionId");
            DropColumn("DMS.Documents", "ClientId");
            DropColumn("DMS.DocumentSubscriptions", "ClientId");
            DropColumn("DMS.DocumentAccesses", "ClientId");
            DropTable("DMS.DocumentEventAccesses");
            CreateIndex("DMS.TemplateDocuments", "DocumentSubjectId");
            CreateIndex("DMS.Documents", "DocumentSubjectId");
            AddForeignKey("DMS.TemplateDocuments", "DocumentSubjectId", "DMS.DictionaryDocumentSubjects", "Id");
            AddForeignKey("DMS.Documents", "DocumentSubjectId", "DMS.DictionaryDocumentSubjects", "Id");
        }
    }
}
