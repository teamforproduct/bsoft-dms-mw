namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DocumentFilesBack : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("DMS.AdminUserRoles", "UserId", "DMS.DictionaryAgents");
            DropForeignKey("DMS.DocumentFiles", "ObjectId", "DMS.SystemObjects");
            DropIndex("DMS.AdminUserRoles", "IX_UserRoleExecutor");
            DropIndex("DMS.AdminUserRoles", new[] { "PositionExecutorId" });
            DropIndex("DMS.DocumentSendLists", new[] { "SourcePositionExecutorAgentId" });
            DropIndex("DMS.DocumentFiles", "IX_EntityObjectNameExtensionVersion");
            DropIndex("DMS.DocumentFiles", "IX_EntityObjectOrderNumberVersion");
            DropIndex("DMS.DocumentFiles", new[] { "DocumentId" });
            DropIndex("DMS.DocumentFiles", new[] { "ClientId" });
            AlterColumn("DMS.AdminUserRoles", "PositionExecutorId", c => c.Int(nullable: false));
            AlterColumn("DMS.DocumentSendLists", "SourcePositionExecutorAgentId", c => c.Int());
            AlterColumn("DMS.DocumentFiles", "Name", c => c.String(maxLength: 2000));
            AlterColumn("DMS.DocumentFiles", "Extension", c => c.String(maxLength: 2000));
            CreateIndex("DMS.AdminUserRoles", new[] { "RoleId", "PositionExecutorId" }, unique: true, name: "IX_UserRoleExecutor");
            CreateIndex("DMS.AdminUserRoles", "PositionExecutorId");
            CreateIndex("DMS.DocumentSendLists", "SourcePositionExecutorAgentId");
            CreateIndex("DMS.DocumentFiles", new[] { "DocumentId", "Name", "Extension", "Version" }, unique: true, name: "IX_DocumentNameExtensionVersion");
            CreateIndex("DMS.DocumentFiles", new[] { "DocumentId", "OrderNumber", "Version" }, unique: true, name: "IX_DocumentOrderNumberVersion");
            DropColumn("DMS.AdminUserRoles", "UserId");
            DropColumn("DMS.DocumentFiles", "EntityId");
            DropColumn("DMS.DocumentFiles", "ObjectId");
            DropColumn("DMS.DocumentFiles", "ClientId");
        }
        
        public override void Down()
        {
            AddColumn("DMS.DocumentFiles", "ClientId", c => c.Int(nullable: false));
            AddColumn("DMS.DocumentFiles", "ObjectId", c => c.Int(nullable: false));
            AddColumn("DMS.DocumentFiles", "EntityId", c => c.Int(nullable: false));
            AddColumn("DMS.AdminUserRoles", "UserId", c => c.Int());
            DropIndex("DMS.DocumentFiles", "IX_DocumentOrderNumberVersion");
            DropIndex("DMS.DocumentFiles", "IX_DocumentNameExtensionVersion");
            DropIndex("DMS.DocumentSendLists", new[] { "SourcePositionExecutorAgentId" });
            DropIndex("DMS.AdminUserRoles", new[] { "PositionExecutorId" });
            DropIndex("DMS.AdminUserRoles", "IX_UserRoleExecutor");
            AlterColumn("DMS.DocumentFiles", "Extension", c => c.String(maxLength: 20));
            AlterColumn("DMS.DocumentFiles", "Name", c => c.String(maxLength: 200));
            AlterColumn("DMS.DocumentSendLists", "SourcePositionExecutorAgentId", c => c.Int(nullable: false));
            AlterColumn("DMS.AdminUserRoles", "PositionExecutorId", c => c.Int());
            CreateIndex("DMS.DocumentFiles", "ClientId");
            CreateIndex("DMS.DocumentFiles", "DocumentId");
            CreateIndex("DMS.DocumentFiles", new[] { "EntityId", "ObjectId", "OrderNumber", "Version" }, unique: true, name: "IX_EntityObjectOrderNumberVersion");
            CreateIndex("DMS.DocumentFiles", new[] { "EntityId", "ObjectId", "Name", "Extension", "Version" }, unique: true, name: "IX_EntityObjectNameExtensionVersion");
            CreateIndex("DMS.DocumentSendLists", "SourcePositionExecutorAgentId");
            CreateIndex("DMS.AdminUserRoles", "PositionExecutorId");
            CreateIndex("DMS.AdminUserRoles", new[] { "UserId", "RoleId", "PositionExecutorId" }, unique: true, name: "IX_UserRoleExecutor");
            AddForeignKey("DMS.DocumentFiles", "ObjectId", "DMS.SystemObjects", "Id");
            AddForeignKey("DMS.AdminUserRoles", "UserId", "DMS.DictionaryAgents", "Id");
        }
    }
}
