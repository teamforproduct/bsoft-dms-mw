namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PrepareForChart : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "DMS.DocumentEventAccessGroups",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClientId = c.Int(nullable: false),
                        EntityTypeId = c.Int(nullable: false),
                        DocumentId = c.Int(nullable: false),
                        EventId = c.Int(nullable: false),
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
                .ForeignKey("DMS.DocumentEvents", t => t.EventId)
                .ForeignKey("DMS.DictionaryPositions", t => t.PositionId)
                .Index(t => t.ClientId)
                .Index(t => t.EntityTypeId)
                .Index(t => t.DocumentId)
                .Index(t => t.EventId)
                .Index(t => t.CompanyId)
                .Index(t => t.DepartmentId)
                .Index(t => t.PositionId)
                .Index(t => t.AgentId);
            
            AddColumn("DMS.DocumentAccesses", "IsAddLater", c => c.Boolean(nullable: false));
            AddColumn("DMS.DocumentAccesses", "IsActive", c => c.Boolean(nullable: false));
            AddColumn("DMS.DocumentAccesses", "CountNewEvents", c => c.Int());
            AddColumn("DMS.DocumentAccesses", "CountWaits", c => c.Int());
            AddColumn("DMS.Documents", "Image", c => c.Binary());
            AddColumn("DMS.DocumentEvents", "IsChanged", c => c.Boolean(nullable: false));
            AddColumn("DMS.DocumentEvents", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("DMS.DocumentFiles", "EventId", c => c.Int());
            AddColumn("DMS.DocumentEventAccesses", "IsFavourite", c => c.Boolean(nullable: false));
            AddColumn("DMS.DocumentEventAccesses", "IsAddLater", c => c.Boolean(nullable: false));
            AddColumn("DMS.DocumentEventAccesses", "IsActive", c => c.Boolean(nullable: false));
            CreateIndex("DMS.DocumentFiles", "EventId");
            AddForeignKey("DMS.DocumentFiles", "EventId", "DMS.DocumentEvents", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("DMS.DocumentFiles", "EventId", "DMS.DocumentEvents");
            DropForeignKey("DMS.DocumentEventAccessGroups", "PositionId", "DMS.DictionaryPositions");
            DropForeignKey("DMS.DocumentEventAccessGroups", "EventId", "DMS.DocumentEvents");
            DropForeignKey("DMS.DocumentEventAccessGroups", "DocumentId", "DMS.Documents");
            DropForeignKey("DMS.DocumentEventAccessGroups", "DepartmentId", "DMS.DictionaryDepartments");
            DropForeignKey("DMS.DocumentEventAccessGroups", "CompanyId", "DMS.DictionaryCompanies");
            DropForeignKey("DMS.DocumentEventAccessGroups", "AgentId", "DMS.DictionaryAgents");
            DropIndex("DMS.DocumentFiles", new[] { "EventId" });
            DropIndex("DMS.DocumentEventAccessGroups", new[] { "AgentId" });
            DropIndex("DMS.DocumentEventAccessGroups", new[] { "PositionId" });
            DropIndex("DMS.DocumentEventAccessGroups", new[] { "DepartmentId" });
            DropIndex("DMS.DocumentEventAccessGroups", new[] { "CompanyId" });
            DropIndex("DMS.DocumentEventAccessGroups", new[] { "EventId" });
            DropIndex("DMS.DocumentEventAccessGroups", new[] { "DocumentId" });
            DropIndex("DMS.DocumentEventAccessGroups", new[] { "EntityTypeId" });
            DropIndex("DMS.DocumentEventAccessGroups", new[] { "ClientId" });
            DropColumn("DMS.DocumentEventAccesses", "IsActive");
            DropColumn("DMS.DocumentEventAccesses", "IsAddLater");
            DropColumn("DMS.DocumentEventAccesses", "IsFavourite");
            DropColumn("DMS.DocumentFiles", "EventId");
            DropColumn("DMS.DocumentEvents", "IsDeleted");
            DropColumn("DMS.DocumentEvents", "IsChanged");
            DropColumn("DMS.Documents", "Image");
            DropColumn("DMS.DocumentAccesses", "CountWaits");
            DropColumn("DMS.DocumentAccesses", "CountNewEvents");
            DropColumn("DMS.DocumentAccesses", "IsActive");
            DropColumn("DMS.DocumentAccesses", "IsAddLater");
            DropTable("DMS.DocumentEventAccessGroups");
        }
    }
}
