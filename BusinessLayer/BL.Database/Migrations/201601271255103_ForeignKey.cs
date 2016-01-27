namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ForeignKey : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Documents", "DocumentDirectionId", "dbo.DictionaryDocumentDirections");
            DropForeignKey("dbo.Documents", "DocumentTypeId", "dbo.DictionaryDocumentTypes");
            DropForeignKey("dbo.Documents", "IncomingDetail_Id", "dbo.DocumentIncomingDetails");
            DropForeignKey("dbo.TemplateDocuments", "IncomingDetail_Id", "dbo.TemplateDocumentIncomingDetails");
            DropIndex("dbo.Documents", new[] { "DocumentDirectionId" });
            DropIndex("dbo.Documents", new[] { "DocumentTypeId" });
            DropIndex("dbo.Documents", new[] { "IncomingDetail_Id" });
            DropIndex("dbo.TemplateDocuments", new[] { "IncomingDetail_Id" });
            DropColumn("dbo.DictionaryPositions", "ParentId");
            DropColumn("dbo.DictionaryDepartments", "ParentId");
            DropColumn("dbo.DictionaryDocumentSubjects", "ParentId");
            DropColumn("dbo.DictionaryStandartSendListContents", "TargetAgentId");
            RenameColumn(table: "dbo.DictionaryPositions", name: "ParentPosition_Id", newName: "ParentId");
            RenameColumn(table: "dbo.DictionaryDepartments", name: "ParentDepartment_Id", newName: "ParentId");
            RenameColumn(table: "dbo.DictionaryDocumentSubjects", name: "ParentDocumentSubject_Id", newName: "ParentId");
            RenameColumn(table: "dbo.DictionaryStandartSendListContents", name: "TargetAgents_Id", newName: "TargetAgentId");
            RenameIndex(table: "dbo.DictionaryPositions", name: "IX_ParentPosition_Id", newName: "IX_ParentId");
            RenameIndex(table: "dbo.DictionaryDepartments", name: "IX_ParentDepartment_Id", newName: "IX_ParentId");
            RenameIndex(table: "dbo.DictionaryStandartSendListContents", name: "IX_TargetAgents_Id", newName: "IX_TargetAgentId");
            RenameIndex(table: "dbo.DictionaryDocumentSubjects", name: "IX_ParentDocumentSubject_Id", newName: "IX_ParentId");
            CreateTable(
                "dbo.DocumentAccesses",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DocumentId = c.Int(nullable: false),
                        PositionId = c.Int(nullable: false),
                        AccessLevelId = c.Int(nullable: false),
                        IsInWork = c.Boolean(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AdminAccessLevels", t => t.AccessLevelId)
                .ForeignKey("dbo.Documents", t => t.DocumentId)
                .ForeignKey("dbo.DictionaryPositions", t => t.PositionId)
                .Index(t => t.DocumentId)
                .Index(t => t.PositionId)
                .Index(t => t.AccessLevelId);
            
            CreateTable(
                "dbo.DocumentEvents",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DocumentId = c.Int(nullable: false),
                        EventTypeId = c.Int(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                        Date = c.DateTime(nullable: false),
                        Description = c.String(),
                        SourcePositionId = c.Int(nullable: false),
                        SourceAgentId = c.Int(nullable: false),
                        TargetPositionId = c.Int(),
                        TargetAgentId = c.Int(),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Documents", t => t.DocumentId)
                .ForeignKey("dbo.DictionaryEventTypes", t => t.EventTypeId)
                .ForeignKey("dbo.DictionaryAgents", t => t.SourceAgentId)
                .ForeignKey("dbo.DictionaryPositions", t => t.SourcePositionId)
                .ForeignKey("dbo.DictionaryAgents", t => t.TargetAgentId)
                .ForeignKey("dbo.DictionaryPositions", t => t.TargetPositionId)
                .Index(t => t.DocumentId)
                .Index(t => t.EventTypeId)
                .Index(t => t.SourcePositionId)
                .Index(t => t.SourceAgentId)
                .Index(t => t.TargetPositionId)
                .Index(t => t.TargetAgentId);
            
            CreateTable(
                "dbo.DocumentSendLists",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DocumentId = c.Int(nullable: false),
                        OrderNumber = c.Int(nullable: false),
                        SendTypeId = c.Int(nullable: false),
                        TargetPositionId = c.Int(),
                        TargetAgentId = c.Int(),
                        Description = c.String(),
                        DueDate = c.DateTime(),
                        DueDay = c.Int(nullable: false),
                        AccessLevelId = c.Int(nullable: false),
                        IsInitial = c.Boolean(nullable: false),
                        EventId = c.Int(),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AdminAccessLevels", t => t.AccessLevelId)
                .ForeignKey("dbo.Documents", t => t.DocumentId)
                .ForeignKey("dbo.DocumentEvents", t => t.EventId)
                .ForeignKey("dbo.DictionarySendTypes", t => t.SendTypeId)
                .ForeignKey("dbo.DictionaryAgents", t => t.TargetAgentId)
                .ForeignKey("dbo.DictionaryPositions", t => t.TargetPositionId)
                .Index(t => t.DocumentId)
                .Index(t => t.SendTypeId)
                .Index(t => t.TargetPositionId)
                .Index(t => t.TargetAgentId)
                .Index(t => t.AccessLevelId)
                .Index(t => t.EventId);
            
            CreateTable(
                "dbo.TemplateDocumentSendLists",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DocumentId = c.Int(nullable: false),
                        SendTypeId = c.Int(nullable: false),
                        TargetPositionId = c.Int(),
                        TargetAgentId = c.Int(),
                        Description = c.String(),
                        DueDate = c.DateTime(),
                        OrderNumber = c.Int(nullable: false),
                        DueDay = c.Int(nullable: false),
                        AccessLevelId = c.Int(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TemplateDocuments", t => t.DocumentId)
                .Index(t => t.DocumentId);
            
            AddColumn("dbo.DictionaryDepartments", "DictionaryPositions_Id", c => c.Int());
            AddColumn("dbo.DocumentIncomingDetails", "DocumentId", c => c.Int(nullable: false));
            AddColumn("dbo.TemplateDocumentIncomingDetails", "DocumentId", c => c.Int(nullable: false));
            CreateIndex("dbo.DictionaryDepartments", "ChiefPositionId");
            CreateIndex("dbo.DictionaryDepartments", "DictionaryPositions_Id");
            CreateIndex("dbo.DocumentIncomingDetails", "SenderAgentId");
            CreateIndex("dbo.TemplateDocumentIncomingDetails", "SenderAgentId");
            AddForeignKey("dbo.DictionaryDepartments", "ChiefPositionId", "dbo.DictionaryPositions", "Id");
            AddForeignKey("dbo.DictionaryDepartments", "DictionaryPositions_Id", "dbo.DictionaryPositions", "Id");
            AddForeignKey("dbo.DocumentIncomingDetails", "SenderAgentId", "dbo.DictionaryAgents", "Id");
            AddForeignKey("dbo.TemplateDocumentIncomingDetails", "SenderAgentId", "dbo.DictionaryAgents", "Id");
            DropColumn("dbo.Documents", "IncomingDetail_Id");
            DropColumn("dbo.TemplateDocuments", "IncomingDetail_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TemplateDocuments", "IncomingDetail_Id", c => c.Int());
            AddColumn("dbo.Documents", "IncomingDetail_Id", c => c.Int());
            DropForeignKey("dbo.TemplateDocumentIncomingDetails", "SenderAgentId", "dbo.DictionaryAgents");
            DropForeignKey("dbo.DocumentIncomingDetails", "SenderAgentId", "dbo.DictionaryAgents");
            DropForeignKey("dbo.TemplateDocumentSendLists", "DocumentId", "dbo.TemplateDocuments");
            DropForeignKey("dbo.DocumentEvents", "TargetPositionId", "dbo.DictionaryPositions");
            DropForeignKey("dbo.DocumentEvents", "TargetAgentId", "dbo.DictionaryAgents");
            DropForeignKey("dbo.DocumentEvents", "SourcePositionId", "dbo.DictionaryPositions");
            DropForeignKey("dbo.DocumentEvents", "SourceAgentId", "dbo.DictionaryAgents");
            DropForeignKey("dbo.DocumentSendLists", "TargetPositionId", "dbo.DictionaryPositions");
            DropForeignKey("dbo.DocumentSendLists", "TargetAgentId", "dbo.DictionaryAgents");
            DropForeignKey("dbo.DocumentSendLists", "SendTypeId", "dbo.DictionarySendTypes");
            DropForeignKey("dbo.DocumentSendLists", "EventId", "dbo.DocumentEvents");
            DropForeignKey("dbo.DocumentSendLists", "DocumentId", "dbo.Documents");
            DropForeignKey("dbo.DocumentSendLists", "AccessLevelId", "dbo.AdminAccessLevels");
            DropForeignKey("dbo.DocumentEvents", "EventTypeId", "dbo.DictionaryEventTypes");
            DropForeignKey("dbo.DocumentEvents", "DocumentId", "dbo.Documents");
            DropForeignKey("dbo.DocumentAccesses", "PositionId", "dbo.DictionaryPositions");
            DropForeignKey("dbo.DocumentAccesses", "DocumentId", "dbo.Documents");
            DropForeignKey("dbo.DocumentAccesses", "AccessLevelId", "dbo.AdminAccessLevels");
            DropForeignKey("dbo.DictionaryDepartments", "DictionaryPositions_Id", "dbo.DictionaryPositions");
            DropForeignKey("dbo.DictionaryDepartments", "ChiefPositionId", "dbo.DictionaryPositions");
            DropIndex("dbo.TemplateDocumentIncomingDetails", new[] { "SenderAgentId" });
            DropIndex("dbo.DocumentIncomingDetails", new[] { "SenderAgentId" });
            DropIndex("dbo.TemplateDocumentSendLists", new[] { "DocumentId" });
            DropIndex("dbo.DocumentSendLists", new[] { "EventId" });
            DropIndex("dbo.DocumentSendLists", new[] { "AccessLevelId" });
            DropIndex("dbo.DocumentSendLists", new[] { "TargetAgentId" });
            DropIndex("dbo.DocumentSendLists", new[] { "TargetPositionId" });
            DropIndex("dbo.DocumentSendLists", new[] { "SendTypeId" });
            DropIndex("dbo.DocumentSendLists", new[] { "DocumentId" });
            DropIndex("dbo.DocumentEvents", new[] { "TargetAgentId" });
            DropIndex("dbo.DocumentEvents", new[] { "TargetPositionId" });
            DropIndex("dbo.DocumentEvents", new[] { "SourceAgentId" });
            DropIndex("dbo.DocumentEvents", new[] { "SourcePositionId" });
            DropIndex("dbo.DocumentEvents", new[] { "EventTypeId" });
            DropIndex("dbo.DocumentEvents", new[] { "DocumentId" });
            DropIndex("dbo.DocumentAccesses", new[] { "AccessLevelId" });
            DropIndex("dbo.DocumentAccesses", new[] { "PositionId" });
            DropIndex("dbo.DocumentAccesses", new[] { "DocumentId" });
            DropIndex("dbo.DictionaryDepartments", new[] { "DictionaryPositions_Id" });
            DropIndex("dbo.DictionaryDepartments", new[] { "ChiefPositionId" });
            DropColumn("dbo.TemplateDocumentIncomingDetails", "DocumentId");
            DropColumn("dbo.DocumentIncomingDetails", "DocumentId");
            DropColumn("dbo.DictionaryDepartments", "DictionaryPositions_Id");
            DropTable("dbo.TemplateDocumentSendLists");
            DropTable("dbo.DocumentSendLists");
            DropTable("dbo.DocumentEvents");
            DropTable("dbo.DocumentAccesses");
            RenameIndex(table: "dbo.DictionaryDocumentSubjects", name: "IX_ParentId", newName: "IX_ParentDocumentSubject_Id");
            RenameIndex(table: "dbo.DictionaryStandartSendListContents", name: "IX_TargetAgentId", newName: "IX_TargetAgents_Id");
            RenameIndex(table: "dbo.DictionaryDepartments", name: "IX_ParentId", newName: "IX_ParentDepartment_Id");
            RenameIndex(table: "dbo.DictionaryPositions", name: "IX_ParentId", newName: "IX_ParentPosition_Id");
            RenameColumn(table: "dbo.DictionaryStandartSendListContents", name: "TargetAgentId", newName: "TargetAgents_Id");
            RenameColumn(table: "dbo.DictionaryDocumentSubjects", name: "ParentId", newName: "ParentDocumentSubject_Id");
            RenameColumn(table: "dbo.DictionaryDepartments", name: "ParentId", newName: "ParentDepartment_Id");
            RenameColumn(table: "dbo.DictionaryPositions", name: "ParentId", newName: "ParentPosition_Id");
            AddColumn("dbo.DictionaryStandartSendListContents", "TargetAgentId", c => c.Int());
            AddColumn("dbo.DictionaryDocumentSubjects", "ParentId", c => c.Int());
            AddColumn("dbo.DictionaryDepartments", "ParentId", c => c.Int());
            AddColumn("dbo.DictionaryPositions", "ParentId", c => c.Int());
            CreateIndex("dbo.TemplateDocuments", "IncomingDetail_Id");
            CreateIndex("dbo.Documents", "IncomingDetail_Id");
            CreateIndex("dbo.Documents", "DocumentTypeId");
            CreateIndex("dbo.Documents", "DocumentDirectionId");
            AddForeignKey("dbo.TemplateDocuments", "IncomingDetail_Id", "dbo.TemplateDocumentIncomingDetails", "Id");
            AddForeignKey("dbo.Documents", "IncomingDetail_Id", "dbo.DocumentIncomingDetails", "Id");
            AddForeignKey("dbo.Documents", "DocumentTypeId", "dbo.DictionaryDocumentTypes", "Id");
            AddForeignKey("dbo.Documents", "DocumentDirectionId", "dbo.DictionaryDocumentDirections", "Id");
        }
    }
}
