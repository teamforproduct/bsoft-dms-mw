using System;
using System.Data.Entity.Migrations;

namespace BL.Database.Migrations
{
   
    public partial class AddDocumentsEnvironmentAndTemplate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AdminAccessLevels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DictionaryDocumentDirections",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(),
                        Name = c.String(),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DictionaryDocumentSubjects",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ParentId = c.Int(),
                        Name = c.String(),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                        ParentDocumentSubject_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DictionaryDocumentSubjects", t => t.ParentDocumentSubject_Id)
                .Index(t => t.ParentDocumentSubject_Id);
            
            CreateTable(
                "dbo.DictionaryDocumentTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        DirectionCodes = c.String(),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DictionaryEventTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        ImpotanceTypeId = c.Int(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                        ImpotanceEventType_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DictionaryImpotanceEventTypes", t => t.ImpotanceEventType_Id)
                .Index(t => t.ImpotanceEventType_Id);
            
            CreateTable(
                "dbo.DictionaryImpotanceEventTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DictionaryRegistrationJournals",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ParentId = c.Int(),
                        Name = c.String(),
                        DepartmentId = c.Int(nullable: false),
                        Index = c.String(),
                        PrefixFormula = c.String(),
                        SuffixFormula = c.String(),
                        DirectionCodes = c.String(),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DictionaryDepartments", t => t.DepartmentId, cascadeDelete: true)
                .Index(t => t.DepartmentId);
            
            CreateTable(
                "dbo.DictionaryStandartSendListContents",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        StandartSendListId = c.Int(nullable: false),
                        OrderNumber = c.Int(nullable: false),
                        SendTypeId = c.Int(nullable: false),
                        TargetPositionId = c.Int(),
                        TargetAgentId = c.Int(),
                        Description = c.String(),
                        DueDate = c.DateTime(),
                        DueDay = c.Int(nullable: false),
                        AccessLevelId = c.Int(),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                        TargetAgents_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AdminAccessLevels", t => t.AccessLevelId)
                .ForeignKey("dbo.DictionarySendTypes", t => t.SendTypeId, cascadeDelete: true)
                .ForeignKey("dbo.DictionaryStandartSendLists", t => t.StandartSendListId, cascadeDelete: true)
                .ForeignKey("dbo.DictionaryAgents", t => t.TargetAgents_Id)
                .ForeignKey("dbo.DictionaryPositions", t => t.TargetPositionId)
                .Index(t => t.StandartSendListId)
                .Index(t => t.SendTypeId)
                .Index(t => t.TargetPositionId)
                .Index(t => t.AccessLevelId)
                .Index(t => t.TargetAgents_Id);
            
            CreateTable(
                "dbo.DictionarySendTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        IsImpotant = c.Boolean(nullable: false),
                        SubordinationTypeId = c.Int(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DictionarySubordinationTypes", t => t.SubordinationTypeId, cascadeDelete: true)
                .Index(t => t.SubordinationTypeId);
            
            CreateTable(
                "dbo.DictionarySubordinationTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DictionaryStandartSendLists",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        PositionId = c.Int(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DictionaryPositions", t => t.PositionId, cascadeDelete: true)
                .Index(t => t.PositionId);
            
            CreateTable(
                "dbo.Files",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DocumentId = c.Int(nullable: false),
                        Name = c.String(),
                        Extention = c.String(),
                        Date = c.DateTime(nullable: false),
                        Content = c.Binary(),
                        IsAdditional = c.Boolean(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Documents", t => t.DocumentId, cascadeDelete: true)
                .Index(t => t.DocumentId);
            
            CreateTable(
                "dbo.Documents",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TemplateDocumentId = c.Int(),
                        DocumentDirectionId = c.Int(nullable: false),
                        DocumentTypeId = c.Int(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                        DocumentSubjectId = c.Int(),
                        Description = c.String(),
                        RegistrationJournalId = c.Int(),
                        RegistrationNumber = c.Int(),
                        RegistrationNumberSuffix = c.String(),
                        RegistrationNumberPrefix = c.String(),
                        RegistrationDate = c.DateTime(),
                        ExecutorPositionId = c.Int(nullable: false),
                        RestrictedSendListId = c.Int(),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                        IncomingDetail_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DictionaryDocumentDirections", t => t.DocumentDirectionId, cascadeDelete: true)
                .ForeignKey("dbo.DictionaryDocumentSubjects", t => t.DocumentSubjectId)
                .ForeignKey("dbo.DictionaryDocumentTypes", t => t.DocumentTypeId, cascadeDelete: true)
                .ForeignKey("dbo.DictionaryPositions", t => t.ExecutorPositionId, cascadeDelete: true)
                .ForeignKey("dbo.DocumentIncomingDetails", t => t.IncomingDetail_Id)
                .ForeignKey("dbo.DictionaryRegistrationJournals", t => t.RegistrationJournalId)
                .ForeignKey("dbo.DictionaryStandartSendLists", t => t.RestrictedSendListId)
                .ForeignKey("dbo.TemplateDocuments", t => t.TemplateDocumentId)
                .Index(t => t.TemplateDocumentId)
                .Index(t => t.DocumentDirectionId)
                .Index(t => t.DocumentTypeId)
                .Index(t => t.DocumentSubjectId)
                .Index(t => t.RegistrationJournalId)
                .Index(t => t.ExecutorPositionId)
                .Index(t => t.RestrictedSendListId)
                .Index(t => t.IncomingDetail_Id);
            
            CreateTable(
                "dbo.DocumentIncomingDetails",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SenderAgentId = c.Int(nullable: false),
                        SenderPerson = c.String(),
                        SenderNumber = c.String(),
                        SenderDate = c.DateTime(nullable: false),
                        Addressee = c.String(),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                        DocumentId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Documents", t => t.DocumentId, cascadeDelete: true)
                .Index(t => t.DocumentId);
            
            CreateTable(
                "dbo.TemplateDocuments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IsHard = c.Int(nullable: false),
                        DocumentSourceId = c.Int(nullable: false),
                        DocumentTypeId = c.Int(nullable: false),
                        DocumentSubjectId = c.Int(),
                        Description = c.String(),
                        RegistrationJournalId = c.Int(),
                        ExecutorPositionId = c.Int(),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                        IncomingDetail_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TemplateDocumentIncomingDetails", t => t.IncomingDetail_Id)
                .Index(t => t.IncomingDetail_Id);
            
            CreateTable(
                "dbo.TemplateDocumentIncomingDetails",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SenderAgentId = c.Int(),
                        SenderPerson = c.String(),
                        SenderNumber = c.String(),
                        SenderDate = c.DateTime(),
                        Addressee = c.String(),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                        DocumentTemplateId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TemplateDocuments", t => t.DocumentTemplateId, cascadeDelete: true)
                .Index(t => t.DocumentTemplateId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Documents", "TemplateDocumentId", "dbo.TemplateDocuments");
            DropForeignKey("dbo.TemplateDocuments", "IncomingDetail_Id", "dbo.TemplateDocumentIncomingDetails");
            DropForeignKey("dbo.TemplateDocumentIncomingDetails", "DocumentTemplateId", "dbo.TemplateDocuments");
            DropForeignKey("dbo.Documents", "RestrictedSendListId", "dbo.DictionaryStandartSendLists");
            DropForeignKey("dbo.Documents", "RegistrationJournalId", "dbo.DictionaryRegistrationJournals");
            DropForeignKey("dbo.Documents", "IncomingDetail_Id", "dbo.DocumentIncomingDetails");
            DropForeignKey("dbo.DocumentIncomingDetails", "DocumentId", "dbo.Documents");
            DropForeignKey("dbo.Files", "DocumentId", "dbo.Documents");
            DropForeignKey("dbo.Documents", "ExecutorPositionId", "dbo.DictionaryPositions");
            DropForeignKey("dbo.Documents", "DocumentTypeId", "dbo.DictionaryDocumentTypes");
            DropForeignKey("dbo.Documents", "DocumentSubjectId", "dbo.DictionaryDocumentSubjects");
            DropForeignKey("dbo.Documents", "DocumentDirectionId", "dbo.DictionaryDocumentDirections");
            DropForeignKey("dbo.DictionaryStandartSendListContents", "TargetPositionId", "dbo.DictionaryPositions");
            DropForeignKey("dbo.DictionaryStandartSendListContents", "TargetAgents_Id", "dbo.DictionaryAgents");
            DropForeignKey("dbo.DictionaryStandartSendListContents", "StandartSendListId", "dbo.DictionaryStandartSendLists");
            DropForeignKey("dbo.DictionaryStandartSendLists", "PositionId", "dbo.DictionaryPositions");
            DropForeignKey("dbo.DictionaryStandartSendListContents", "SendTypeId", "dbo.DictionarySendTypes");
            DropForeignKey("dbo.DictionarySendTypes", "SubordinationTypeId", "dbo.DictionarySubordinationTypes");
            DropForeignKey("dbo.DictionaryStandartSendListContents", "AccessLevelId", "dbo.AdminAccessLevels");
            DropForeignKey("dbo.DictionaryRegistrationJournals", "DepartmentId", "dbo.DictionaryDepartments");
            DropForeignKey("dbo.DictionaryEventTypes", "ImpotanceEventType_Id", "dbo.DictionaryImpotanceEventTypes");
            DropForeignKey("dbo.DictionaryDocumentSubjects", "ParentDocumentSubject_Id", "dbo.DictionaryDocumentSubjects");
            DropIndex("dbo.TemplateDocumentIncomingDetails", new[] { "DocumentTemplateId" });
            DropIndex("dbo.TemplateDocuments", new[] { "IncomingDetail_Id" });
            DropIndex("dbo.DocumentIncomingDetails", new[] { "DocumentId" });
            DropIndex("dbo.Documents", new[] { "IncomingDetail_Id" });
            DropIndex("dbo.Documents", new[] { "RestrictedSendListId" });
            DropIndex("dbo.Documents", new[] { "ExecutorPositionId" });
            DropIndex("dbo.Documents", new[] { "RegistrationJournalId" });
            DropIndex("dbo.Documents", new[] { "DocumentSubjectId" });
            DropIndex("dbo.Documents", new[] { "DocumentTypeId" });
            DropIndex("dbo.Documents", new[] { "DocumentDirectionId" });
            DropIndex("dbo.Documents", new[] { "TemplateDocumentId" });
            DropIndex("dbo.Files", new[] { "DocumentId" });
            DropIndex("dbo.DictionaryStandartSendLists", new[] { "PositionId" });
            DropIndex("dbo.DictionarySendTypes", new[] { "SubordinationTypeId" });
            DropIndex("dbo.DictionaryStandartSendListContents", new[] { "TargetAgents_Id" });
            DropIndex("dbo.DictionaryStandartSendListContents", new[] { "AccessLevelId" });
            DropIndex("dbo.DictionaryStandartSendListContents", new[] { "TargetPositionId" });
            DropIndex("dbo.DictionaryStandartSendListContents", new[] { "SendTypeId" });
            DropIndex("dbo.DictionaryStandartSendListContents", new[] { "StandartSendListId" });
            DropIndex("dbo.DictionaryRegistrationJournals", new[] { "DepartmentId" });
            DropIndex("dbo.DictionaryEventTypes", new[] { "ImpotanceEventType_Id" });
            DropIndex("dbo.DictionaryDocumentSubjects", new[] { "ParentDocumentSubject_Id" });
            DropTable("dbo.TemplateDocumentIncomingDetails");
            DropTable("dbo.TemplateDocuments");
            DropTable("dbo.DocumentIncomingDetails");
            DropTable("dbo.Documents");
            DropTable("dbo.Files");
            DropTable("dbo.DictionaryStandartSendLists");
            DropTable("dbo.DictionarySubordinationTypes");
            DropTable("dbo.DictionarySendTypes");
            DropTable("dbo.DictionaryStandartSendListContents");
            DropTable("dbo.DictionaryRegistrationJournals");
            DropTable("dbo.DictionaryImpotanceEventTypes");
            DropTable("dbo.DictionaryEventTypes");
            DropTable("dbo.DictionaryDocumentTypes");
            DropTable("dbo.DictionaryDocumentSubjects");
            DropTable("dbo.DictionaryDocumentDirections");
            DropTable("dbo.AdminAccessLevels");
        }
    }
}
