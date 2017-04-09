namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixAccessGroups : DbMigration
    {
        public override void Up()
        {
            DropIndex("DMS.DocumentEvents", "IX_IsAvailableWithinTask");
            CreateTable(
                "DMS.DocumentSendListAccessGroups",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClientId = c.Int(nullable: false),
                        EntityTypeId = c.Int(nullable: false),
                        DocumentId = c.Int(nullable: false),
                        SendListId = c.Int(nullable: false),
                        AccessTypeId = c.Int(nullable: false),
                        AccessGroupsTypeId = c.Int(nullable: false),
                        CompanyId = c.Int(),
                        DepartmentId = c.Int(),
                        PositionId = c.Int(),
                        AgentId = c.Int(),
                        IsActive = c.Boolean(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.DictionaryAgents", t => t.AgentId)
                .ForeignKey("DMS.DictionaryCompanies", t => t.CompanyId)
                .ForeignKey("DMS.DictionaryDepartments", t => t.DepartmentId)
                .ForeignKey("DMS.Documents", t => t.DocumentId)
                .ForeignKey("DMS.DictionaryPositions", t => t.PositionId)
                .ForeignKey("DMS.DocumentSendLists", t => t.SendListId)
                .Index(t => t.ClientId)
                .Index(t => t.EntityTypeId)
                .Index(t => t.DocumentId)
                .Index(t => t.SendListId)
                .Index(t => t.CompanyId)
                .Index(t => t.DepartmentId)
                .Index(t => t.PositionId)
                .Index(t => t.AgentId);
            
            CreateTable(
                "DMS.TemplateDocumentSendListAccessGroups",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DocumentId = c.Int(nullable: false),
                        SendListId = c.Int(nullable: false),
                        AccessTypeId = c.Int(nullable: false),
                        AccessGroupsTypeId = c.Int(nullable: false),
                        CompanyId = c.Int(),
                        DepartmentId = c.Int(),
                        PositionId = c.Int(),
                        AgentId = c.Int(),
                        IsActive = c.Boolean(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.DictionaryAgents", t => t.AgentId)
                .ForeignKey("DMS.DictionaryCompanies", t => t.CompanyId)
                .ForeignKey("DMS.DictionaryDepartments", t => t.DepartmentId)
                .ForeignKey("DMS.TemplateDocuments", t => t.DocumentId)
                .ForeignKey("DMS.DictionaryPositions", t => t.PositionId)
                .ForeignKey("DMS.TemplateDocumentSendLists", t => t.SendListId)
                .Index(t => t.DocumentId)
                .Index(t => t.SendListId)
                .Index(t => t.CompanyId)
                .Index(t => t.DepartmentId)
                .Index(t => t.PositionId)
                .Index(t => t.AgentId);
            
            AddColumn("DMS.DocumentEventAccesses", "PositionExecutorTypeId", c => c.Int());
            DropColumn("DMS.DocumentEvents", "IsAvailableWithinTask");
            DropColumn("DMS.DocumentSendLists", "IsAvailableWithinTask");
            DropColumn("DMS.TemplateDocumentSendLists", "IsAvailableWithinTask");
        }
        
        public override void Down()
        {
            AddColumn("DMS.TemplateDocumentSendLists", "IsAvailableWithinTask", c => c.Boolean(nullable: false));
            AddColumn("DMS.DocumentSendLists", "IsAvailableWithinTask", c => c.Boolean(nullable: false));
            AddColumn("DMS.DocumentEvents", "IsAvailableWithinTask", c => c.Boolean(nullable: false));
            DropForeignKey("DMS.TemplateDocumentSendListAccessGroups", "SendListId", "DMS.TemplateDocumentSendLists");
            DropForeignKey("DMS.TemplateDocumentSendListAccessGroups", "PositionId", "DMS.DictionaryPositions");
            DropForeignKey("DMS.TemplateDocumentSendListAccessGroups", "DocumentId", "DMS.TemplateDocuments");
            DropForeignKey("DMS.TemplateDocumentSendListAccessGroups", "DepartmentId", "DMS.DictionaryDepartments");
            DropForeignKey("DMS.TemplateDocumentSendListAccessGroups", "CompanyId", "DMS.DictionaryCompanies");
            DropForeignKey("DMS.TemplateDocumentSendListAccessGroups", "AgentId", "DMS.DictionaryAgents");
            DropForeignKey("DMS.DocumentSendListAccessGroups", "SendListId", "DMS.DocumentSendLists");
            DropForeignKey("DMS.DocumentSendListAccessGroups", "PositionId", "DMS.DictionaryPositions");
            DropForeignKey("DMS.DocumentSendListAccessGroups", "DocumentId", "DMS.Documents");
            DropForeignKey("DMS.DocumentSendListAccessGroups", "DepartmentId", "DMS.DictionaryDepartments");
            DropForeignKey("DMS.DocumentSendListAccessGroups", "CompanyId", "DMS.DictionaryCompanies");
            DropForeignKey("DMS.DocumentSendListAccessGroups", "AgentId", "DMS.DictionaryAgents");
            DropIndex("DMS.TemplateDocumentSendListAccessGroups", new[] { "AgentId" });
            DropIndex("DMS.TemplateDocumentSendListAccessGroups", new[] { "PositionId" });
            DropIndex("DMS.TemplateDocumentSendListAccessGroups", new[] { "DepartmentId" });
            DropIndex("DMS.TemplateDocumentSendListAccessGroups", new[] { "CompanyId" });
            DropIndex("DMS.TemplateDocumentSendListAccessGroups", new[] { "SendListId" });
            DropIndex("DMS.TemplateDocumentSendListAccessGroups", new[] { "DocumentId" });
            DropIndex("DMS.DocumentSendListAccessGroups", new[] { "AgentId" });
            DropIndex("DMS.DocumentSendListAccessGroups", new[] { "PositionId" });
            DropIndex("DMS.DocumentSendListAccessGroups", new[] { "DepartmentId" });
            DropIndex("DMS.DocumentSendListAccessGroups", new[] { "CompanyId" });
            DropIndex("DMS.DocumentSendListAccessGroups", new[] { "SendListId" });
            DropIndex("DMS.DocumentSendListAccessGroups", new[] { "DocumentId" });
            DropIndex("DMS.DocumentSendListAccessGroups", new[] { "EntityTypeId" });
            DropIndex("DMS.DocumentSendListAccessGroups", new[] { "ClientId" });
            DropColumn("DMS.DocumentEventAccesses", "PositionExecutorTypeId");
            DropTable("DMS.TemplateDocumentSendListAccessGroups");
            DropTable("DMS.DocumentSendListAccessGroups");
            CreateIndex("DMS.DocumentEvents", new[] { "IsAvailableWithinTask", "TaskId" }, name: "IX_IsAvailableWithinTask");
        }
    }
}
