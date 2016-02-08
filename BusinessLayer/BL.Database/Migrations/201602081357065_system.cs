namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class system : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.DocumentTemporaryRegistrations", "RegistrationJournalId", "dbo.DictionaryRegistrationJournals");
            DropForeignKey("dbo.DocumentTemporaryRegistrations", "Id", "dbo.Documents");
            DropIndex("dbo.DocumentTemporaryRegistrations", new[] { "Id" });
            DropIndex("dbo.DocumentTemporaryRegistrations", new[] { "RegistrationJournalId" });
            CreateTable(
                "dbo.AdminRoleActions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RoleId = c.Int(nullable: false),
                        ActionId = c.Int(nullable: false),
                        RecordId = c.Int(),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.SystemActions", t => t.ActionId)
                .ForeignKey("dbo.AdminRoles", t => t.RoleId)
                .Index(t => t.RoleId)
                .Index(t => t.ActionId);
            
            CreateTable(
                "dbo.SystemActions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ObjectId = c.Int(nullable: false),
                        Code = c.String(),
                        API = c.String(),
                        Description = c.String(),
                        IsGrantable = c.Boolean(nullable: false),
                        IsGrantableByRecordId = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.SystemObjects", t => t.ObjectId)
                .Index(t => t.ObjectId);
            
            CreateTable(
                "dbo.SystemObjects",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SystemFields",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ObjectId = c.Int(nullable: false),
                        Code = c.String(),
                        Description = c.String(),
                        ValueTypeId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.SystemObjects", t => t.ObjectId)
                .ForeignKey("dbo.SystemValueTypes", t => t.ValueTypeId)
                .Index(t => t.ObjectId)
                .Index(t => t.ValueTypeId);
            
            CreateTable(
                "dbo.SystemValueTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AdminRoles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        PositionId = c.Int(nullable: false),
                        AccessLevelId = c.Int(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AdminAccessLevels", t => t.AccessLevelId)
                .ForeignKey("dbo.DictionaryPositions", t => t.PositionId)
                .Index(t => t.PositionId)
                .Index(t => t.AccessLevelId);
            
            CreateTable(
                "dbo.AdminUserRoles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        RoleId = c.Int(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DictionaryAgents", t => t.UserId)
                .ForeignKey("dbo.AdminRoles", t => t.RoleId)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            DropTable("dbo.DocumentTemporaryRegistrations");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.DocumentTemporaryRegistrations",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        RegistrationJournalId = c.Int(nullable: false),
                        RegistrationNumber = c.Int(nullable: false),
                        RegistrationNumberSuffix = c.String(),
                        RegistrationNumberPrefix = c.String(),
                        RegistrationDate = c.DateTime(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            DropForeignKey("dbo.AdminUserRoles", "RoleId", "dbo.AdminRoles");
            DropForeignKey("dbo.AdminUserRoles", "UserId", "dbo.DictionaryAgents");
            DropForeignKey("dbo.AdminRoleActions", "RoleId", "dbo.AdminRoles");
            DropForeignKey("dbo.AdminRoles", "PositionId", "dbo.DictionaryPositions");
            DropForeignKey("dbo.AdminRoles", "AccessLevelId", "dbo.AdminAccessLevels");
            DropForeignKey("dbo.AdminRoleActions", "ActionId", "dbo.SystemActions");
            DropForeignKey("dbo.SystemFields", "ValueTypeId", "dbo.SystemValueTypes");
            DropForeignKey("dbo.SystemFields", "ObjectId", "dbo.SystemObjects");
            DropForeignKey("dbo.SystemActions", "ObjectId", "dbo.SystemObjects");
            DropIndex("dbo.AdminUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AdminUserRoles", new[] { "UserId" });
            DropIndex("dbo.AdminRoles", new[] { "AccessLevelId" });
            DropIndex("dbo.AdminRoles", new[] { "PositionId" });
            DropIndex("dbo.SystemFields", new[] { "ValueTypeId" });
            DropIndex("dbo.SystemFields", new[] { "ObjectId" });
            DropIndex("dbo.SystemActions", new[] { "ObjectId" });
            DropIndex("dbo.AdminRoleActions", new[] { "ActionId" });
            DropIndex("dbo.AdminRoleActions", new[] { "RoleId" });
            DropTable("dbo.AdminUserRoles");
            DropTable("dbo.AdminRoles");
            DropTable("dbo.SystemValueTypes");
            DropTable("dbo.SystemFields");
            DropTable("dbo.SystemObjects");
            DropTable("dbo.SystemActions");
            DropTable("dbo.AdminRoleActions");
            CreateIndex("dbo.DocumentTemporaryRegistrations", "RegistrationJournalId");
            CreateIndex("dbo.DocumentTemporaryRegistrations", "Id");
            AddForeignKey("dbo.DocumentTemporaryRegistrations", "Id", "dbo.Documents", "Id");
            AddForeignKey("dbo.DocumentTemporaryRegistrations", "RegistrationJournalId", "dbo.DictionaryRegistrationJournals", "Id");
        }
    }
}
