namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dictionaries : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DictionaryPositions", "FullName", c => c.String());
            AddColumn("dbo.DictionaryPositions", "MainExecutorAgentId", c => c.Int());
            AddColumn("dbo.DictionaryPositions", "IsActive", c => c.Boolean(nullable: false));
            AddColumn("dbo.DictionaryDepartments", "Code", c => c.String());
            AddColumn("dbo.DictionaryDepartments", "FullName", c => c.String());
            AddColumn("dbo.DictionaryDepartments", "IsActive", c => c.Boolean(nullable: false));
            AddColumn("dbo.DictionaryCompanies", "IsActive", c => c.Boolean(nullable: false));
            AddColumn("dbo.DictionaryTags", "IsActive", c => c.Boolean(nullable: false));
            AddColumn("dbo.DictionaryDocumentSubjects", "IsActive", c => c.Boolean(nullable: false));
            AddColumn("dbo.DictionaryEventTypes", "SourceDescription", c => c.String());
            AddColumn("dbo.DictionaryEventTypes", "TargetDescription", c => c.String());
            AddColumn("dbo.DictionaryResultTypes", "IsActive", c => c.Boolean(nullable: false));
            AddColumn("dbo.DictionaryLinkTypes", "IsActive", c => c.Boolean(nullable: false));
            AddColumn("dbo.DictionaryRegistrationJournals", "IsActive", c => c.Boolean(nullable: false));
            AddColumn("dbo.TemplateDocuments", "IsActive", c => c.Boolean(nullable: false));
            AddColumn("dbo.DictionaryDocumentTypes", "IsActive", c => c.Boolean(nullable: false));
            AddColumn("dbo.CustomDictionaries", "IsActive", c => c.Boolean(nullable: false));
            CreateIndex("dbo.DictionaryPositions", "MainExecutorAgentId");
            AddForeignKey("dbo.DictionaryPositions", "MainExecutorAgentId", "dbo.DictionaryAgents", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DictionaryPositions", "MainExecutorAgentId", "dbo.DictionaryAgents");
            DropIndex("dbo.DictionaryPositions", new[] { "MainExecutorAgentId" });
            DropColumn("dbo.CustomDictionaries", "IsActive");
            DropColumn("dbo.DictionaryDocumentTypes", "IsActive");
            DropColumn("dbo.TemplateDocuments", "IsActive");
            DropColumn("dbo.DictionaryRegistrationJournals", "IsActive");
            DropColumn("dbo.DictionaryLinkTypes", "IsActive");
            DropColumn("dbo.DictionaryResultTypes", "IsActive");
            DropColumn("dbo.DictionaryEventTypes", "TargetDescription");
            DropColumn("dbo.DictionaryEventTypes", "SourceDescription");
            DropColumn("dbo.DictionaryDocumentSubjects", "IsActive");
            DropColumn("dbo.DictionaryTags", "IsActive");
            DropColumn("dbo.DictionaryCompanies", "IsActive");
            DropColumn("dbo.DictionaryDepartments", "IsActive");
            DropColumn("dbo.DictionaryDepartments", "FullName");
            DropColumn("dbo.DictionaryDepartments", "Code");
            DropColumn("dbo.DictionaryPositions", "IsActive");
            DropColumn("dbo.DictionaryPositions", "MainExecutorAgentId");
            DropColumn("dbo.DictionaryPositions", "FullName");
        }
    }
}
