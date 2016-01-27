namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Corrections : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.DocumentIncomingDetails", "DocumentId", "dbo.Documents");
            DropForeignKey("dbo.TemplateDocumentIncomingDetails", "DocumentTemplateId", "dbo.TemplateDocuments");
            DropForeignKey("dbo.DictionaryPositions", "DepartmentId", "dbo.DictionaryDepartments");
            DropForeignKey("dbo.DictionaryDepartments", "CompanyId", "dbo.DictionaryCompanies");
            DropForeignKey("dbo.DictionaryRegistrationJournals", "DepartmentId", "dbo.DictionaryDepartments");
            DropForeignKey("dbo.DictionaryStandartSendListContents", "SendTypeId", "dbo.DictionarySendTypes");
            DropForeignKey("dbo.DictionaryStandartSendListContents", "StandartSendListId", "dbo.DictionaryStandartSendLists");
            DropForeignKey("dbo.DictionarySendTypes", "SubordinationTypeId", "dbo.DictionarySubordinationTypes");
            DropForeignKey("dbo.DictionaryStandartSendLists", "PositionId", "dbo.DictionaryPositions");
            DropForeignKey("dbo.DocumentFiles", "DocumentId", "dbo.Documents");
            DropForeignKey("dbo.Documents", "DocumentDirectionId", "dbo.DictionaryDocumentDirections");
            DropForeignKey("dbo.Documents", "DocumentTypeId", "dbo.DictionaryDocumentTypes");
            DropForeignKey("dbo.Documents", "ExecutorPositionId", "dbo.DictionaryPositions");
            DropIndex("dbo.DictionaryEventTypes", new[] { "ImpotanceEventType_Id" });
            DropIndex("dbo.Documents", new[] { "TemplateDocumentId" });
            DropIndex("dbo.DocumentIncomingDetails", new[] { "DocumentId" });
            DropIndex("dbo.TemplateDocumentIncomingDetails", new[] { "DocumentTemplateId" });
            RenameColumn(table: "dbo.DictionaryEventTypes", name: "ImpotanceEventType_Id", newName: "ImpotanceEventTypeId");
            AddColumn("dbo.Documents", "ExecutorAgentId", c => c.Int(nullable: false));
            AddColumn("dbo.TemplateDocuments", "Name", c => c.String());
            AddColumn("dbo.TemplateDocuments", "DocumentDirectionId", c => c.Int(nullable: false));
            AddColumn("dbo.TemplateDocuments", "RestrictedSendListId", c => c.Int());
            AlterColumn("dbo.DictionaryEventTypes", "ImpotanceEventTypeId", c => c.Int(nullable: false));
            AlterColumn("dbo.Documents", "TemplateDocumentId", c => c.Int(nullable: false));
            CreateIndex("dbo.DictionaryEventTypes", "ImpotanceEventTypeId");
            CreateIndex("dbo.Documents", "TemplateDocumentId");
            CreateIndex("dbo.Documents", "ExecutorAgentId");
            CreateIndex("dbo.TemplateDocuments", "DocumentDirectionId");
            CreateIndex("dbo.TemplateDocuments", "DocumentTypeId");
            CreateIndex("dbo.TemplateDocuments", "DocumentSubjectId");
            CreateIndex("dbo.TemplateDocuments", "RegistrationJournalId");
            CreateIndex("dbo.TemplateDocuments", "RestrictedSendListId");
            AddForeignKey("dbo.Documents", "ExecutorAgentId", "dbo.DictionaryAgents", "Id");
            AddForeignKey("dbo.TemplateDocuments", "DocumentDirectionId", "dbo.DictionaryDocumentDirections", "Id");
            AddForeignKey("dbo.TemplateDocuments", "DocumentSubjectId", "dbo.DictionaryDocumentSubjects", "Id");
            AddForeignKey("dbo.TemplateDocuments", "DocumentTypeId", "dbo.DictionaryDocumentTypes", "Id");
            AddForeignKey("dbo.TemplateDocuments", "RegistrationJournalId", "dbo.DictionaryRegistrationJournals", "Id");
            AddForeignKey("dbo.TemplateDocuments", "RestrictedSendListId", "dbo.DictionaryStandartSendLists", "Id");
            AddForeignKey("dbo.DictionaryPositions", "DepartmentId", "dbo.DictionaryDepartments", "Id");
            AddForeignKey("dbo.DictionaryDepartments", "CompanyId", "dbo.DictionaryCompanies", "Id");
            AddForeignKey("dbo.DictionaryRegistrationJournals", "DepartmentId", "dbo.DictionaryDepartments", "Id");
            AddForeignKey("dbo.DictionaryStandartSendListContents", "SendTypeId", "dbo.DictionarySendTypes", "Id");
            AddForeignKey("dbo.DictionaryStandartSendListContents", "StandartSendListId", "dbo.DictionaryStandartSendLists", "Id");
            AddForeignKey("dbo.DictionarySendTypes", "SubordinationTypeId", "dbo.DictionarySubordinationTypes", "Id");
            AddForeignKey("dbo.DictionaryStandartSendLists", "PositionId", "dbo.DictionaryPositions", "Id");
            AddForeignKey("dbo.DocumentFiles", "DocumentId", "dbo.Documents", "Id");
            AddForeignKey("dbo.Documents", "DocumentDirectionId", "dbo.DictionaryDocumentDirections", "Id");
            AddForeignKey("dbo.Documents", "DocumentTypeId", "dbo.DictionaryDocumentTypes", "Id");
            AddForeignKey("dbo.Documents", "ExecutorPositionId", "dbo.DictionaryPositions", "Id");
            DropColumn("dbo.DictionaryEventTypes", "ImpotanceTypeId");
            DropColumn("dbo.DictionaryRegistrationJournals", "ParentId");
            DropColumn("dbo.DocumentIncomingDetails", "DocumentId");
            DropColumn("dbo.TemplateDocuments", "DocumentSourceId");
            DropColumn("dbo.TemplateDocuments", "ExecutorPositionId");
            DropColumn("dbo.TemplateDocumentIncomingDetails", "DocumentTemplateId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TemplateDocumentIncomingDetails", "DocumentTemplateId", c => c.Int(nullable: false));
            AddColumn("dbo.TemplateDocuments", "ExecutorPositionId", c => c.Int());
            AddColumn("dbo.TemplateDocuments", "DocumentSourceId", c => c.Int(nullable: false));
            AddColumn("dbo.DocumentIncomingDetails", "DocumentId", c => c.Int(nullable: false));
            AddColumn("dbo.DictionaryRegistrationJournals", "ParentId", c => c.Int());
            AddColumn("dbo.DictionaryEventTypes", "ImpotanceTypeId", c => c.Int(nullable: false));
            DropForeignKey("dbo.Documents", "ExecutorPositionId", "dbo.DictionaryPositions");
            DropForeignKey("dbo.Documents", "DocumentTypeId", "dbo.DictionaryDocumentTypes");
            DropForeignKey("dbo.Documents", "DocumentDirectionId", "dbo.DictionaryDocumentDirections");
            DropForeignKey("dbo.DocumentFiles", "DocumentId", "dbo.Documents");
            DropForeignKey("dbo.DictionaryStandartSendLists", "PositionId", "dbo.DictionaryPositions");
            DropForeignKey("dbo.DictionarySendTypes", "SubordinationTypeId", "dbo.DictionarySubordinationTypes");
            DropForeignKey("dbo.DictionaryStandartSendListContents", "StandartSendListId", "dbo.DictionaryStandartSendLists");
            DropForeignKey("dbo.DictionaryStandartSendListContents", "SendTypeId", "dbo.DictionarySendTypes");
            DropForeignKey("dbo.DictionaryRegistrationJournals", "DepartmentId", "dbo.DictionaryDepartments");
            DropForeignKey("dbo.DictionaryDepartments", "CompanyId", "dbo.DictionaryCompanies");
            DropForeignKey("dbo.DictionaryPositions", "DepartmentId", "dbo.DictionaryDepartments");
            DropForeignKey("dbo.TemplateDocuments", "RestrictedSendListId", "dbo.DictionaryStandartSendLists");
            DropForeignKey("dbo.TemplateDocuments", "RegistrationJournalId", "dbo.DictionaryRegistrationJournals");
            DropForeignKey("dbo.TemplateDocuments", "DocumentTypeId", "dbo.DictionaryDocumentTypes");
            DropForeignKey("dbo.TemplateDocuments", "DocumentSubjectId", "dbo.DictionaryDocumentSubjects");
            DropForeignKey("dbo.TemplateDocuments", "DocumentDirectionId", "dbo.DictionaryDocumentDirections");
            DropForeignKey("dbo.Documents", "ExecutorAgentId", "dbo.DictionaryAgents");
            DropIndex("dbo.TemplateDocuments", new[] { "RestrictedSendListId" });
            DropIndex("dbo.TemplateDocuments", new[] { "RegistrationJournalId" });
            DropIndex("dbo.TemplateDocuments", new[] { "DocumentSubjectId" });
            DropIndex("dbo.TemplateDocuments", new[] { "DocumentTypeId" });
            DropIndex("dbo.TemplateDocuments", new[] { "DocumentDirectionId" });
            DropIndex("dbo.Documents", new[] { "ExecutorAgentId" });
            DropIndex("dbo.Documents", new[] { "TemplateDocumentId" });
            DropIndex("dbo.DictionaryEventTypes", new[] { "ImpotanceEventTypeId" });
            AlterColumn("dbo.Documents", "TemplateDocumentId", c => c.Int());
            AlterColumn("dbo.DictionaryEventTypes", "ImpotanceEventTypeId", c => c.Int());
            DropColumn("dbo.TemplateDocuments", "RestrictedSendListId");
            DropColumn("dbo.TemplateDocuments", "DocumentDirectionId");
            DropColumn("dbo.TemplateDocuments", "Name");
            DropColumn("dbo.Documents", "ExecutorAgentId");
            RenameColumn(table: "dbo.DictionaryEventTypes", name: "ImpotanceEventTypeId", newName: "ImpotanceEventType_Id");
            CreateIndex("dbo.TemplateDocumentIncomingDetails", "DocumentTemplateId");
            CreateIndex("dbo.DocumentIncomingDetails", "DocumentId");
            CreateIndex("dbo.Documents", "TemplateDocumentId");
            CreateIndex("dbo.DictionaryEventTypes", "ImpotanceEventType_Id");
            AddForeignKey("dbo.Documents", "ExecutorPositionId", "dbo.DictionaryPositions", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Documents", "DocumentTypeId", "dbo.DictionaryDocumentTypes", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Documents", "DocumentDirectionId", "dbo.DictionaryDocumentDirections", "Id", cascadeDelete: true);
            AddForeignKey("dbo.DocumentFiles", "DocumentId", "dbo.Documents", "Id", cascadeDelete: true);
            AddForeignKey("dbo.DictionaryStandartSendLists", "PositionId", "dbo.DictionaryPositions", "Id", cascadeDelete: true);
            AddForeignKey("dbo.DictionarySendTypes", "SubordinationTypeId", "dbo.DictionarySubordinationTypes", "Id", cascadeDelete: true);
            AddForeignKey("dbo.DictionaryStandartSendListContents", "StandartSendListId", "dbo.DictionaryStandartSendLists", "Id", cascadeDelete: true);
            AddForeignKey("dbo.DictionaryStandartSendListContents", "SendTypeId", "dbo.DictionarySendTypes", "Id", cascadeDelete: true);
            AddForeignKey("dbo.DictionaryRegistrationJournals", "DepartmentId", "dbo.DictionaryDepartments", "Id", cascadeDelete: true);
            AddForeignKey("dbo.DictionaryDepartments", "CompanyId", "dbo.DictionaryCompanies", "Id", cascadeDelete: true);
            AddForeignKey("dbo.DictionaryPositions", "DepartmentId", "dbo.DictionaryDepartments", "Id", cascadeDelete: true);
            AddForeignKey("dbo.TemplateDocumentIncomingDetails", "DocumentTemplateId", "dbo.TemplateDocuments", "Id", cascadeDelete: true);
            AddForeignKey("dbo.DocumentIncomingDetails", "DocumentId", "dbo.Documents", "Id", cascadeDelete: true);
        }
    }
}
