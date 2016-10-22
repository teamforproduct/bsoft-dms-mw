namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NewUserRoles_And_SystemLogs : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("DMS.AdminUserRoles", "PositionId", "DMS.DictionaryPositions");
            DropIndex("DMS.DictionaryAgentAccounts", new[] { "AgentBankId" });
            DropIndex("DMS.DictionaryAgentUsers", new[] { "UserId" });
            DropIndex("DMS.AdminUserRoles", "IX_UserRoleStartDate");
            DropIndex("DMS.AdminUserRoles", new[] { "PositionId" });
            RenameColumn(table: "DMS.DictionaryAgentAccounts", name: "AgentBankId", newName: "AgentBank_Id");
            AddColumn("DMS.DictionaryAgentUsers", "ClientId", c => c.Int(nullable: false));
            AddColumn("DMS.DictionaryAgentUsers", "IsActive", c => c.Boolean(nullable: false));
            AddColumn("DMS.SystemLogs", "ObjectLog", c => c.String(maxLength: 2000));
            AlterColumn("DMS.DictionaryAgentAccounts", "AgentBank_Id", c => c.Int());
            AlterColumn("DMS.AdminUserRoles", "UserId", c => c.Int());
            CreateIndex("DMS.DictionaryAgentAccounts", "AgentBank_Id");
            CreateIndex("DMS.DictionaryAgentUsers", "ClientId");
            CreateIndex("DMS.DictionaryAgentUsers", new[] { "UserId", "ClientId" }, unique: true, name: "IX_UserId");
            CreateIndex("DMS.AdminUserRoles", new[] { "UserId", "RoleId", "PositionExecutorId" }, unique: true, name: "IX_UserRoleExecutor");
            CreateIndex("DMS.AdminUserRoles", "PositionExecutorId");
            CreateIndex("DMS.SystemLogs", "ExecutorAgentId");
            AddForeignKey("DMS.SystemLogs", "ExecutorAgentId", "DMS.DictionaryAgents", "Id");
            DropColumn("DMS.DictionaryAgentUsers", "Login");
            DropColumn("DMS.DictionaryAgentUsers", "PasswordHash");
            DropColumn("DMS.AdminUserRoles", "StartDate");
            DropColumn("DMS.AdminUserRoles", "PositionId");
            DropColumn("DMS.AdminUserRoles", "EndDate");
        }
        
        public override void Down()
        {
            AddColumn("DMS.AdminUserRoles", "EndDate", c => c.DateTime(nullable: false));
            AddColumn("DMS.AdminUserRoles", "PositionId", c => c.Int());
            AddColumn("DMS.AdminUserRoles", "StartDate", c => c.DateTime(nullable: false));
            AddColumn("DMS.DictionaryAgentUsers", "PasswordHash", c => c.String(maxLength: 2000));
            AddColumn("DMS.DictionaryAgentUsers", "Login", c => c.String(maxLength: 256));
            DropForeignKey("DMS.SystemLogs", "ExecutorAgentId", "DMS.DictionaryAgents");
            DropIndex("DMS.SystemLogs", new[] { "ExecutorAgentId" });
            DropIndex("DMS.AdminUserRoles", new[] { "PositionExecutorId" });
            DropIndex("DMS.AdminUserRoles", "IX_UserRoleExecutor");
            DropIndex("DMS.DictionaryAgentUsers", "IX_UserId");
            DropIndex("DMS.DictionaryAgentUsers", new[] { "ClientId" });
            DropIndex("DMS.DictionaryAgentAccounts", new[] { "AgentBank_Id" });
            AlterColumn("DMS.AdminUserRoles", "UserId", c => c.Int(nullable: false));
            AlterColumn("DMS.DictionaryAgentAccounts", "AgentBank_Id", c => c.Int(nullable: false));
            DropColumn("DMS.SystemLogs", "ObjectLog");
            DropColumn("DMS.DictionaryAgentUsers", "IsActive");
            DropColumn("DMS.DictionaryAgentUsers", "ClientId");
            RenameColumn(table: "DMS.DictionaryAgentAccounts", name: "AgentBank_Id", newName: "AgentBankId");
            CreateIndex("DMS.AdminUserRoles", "PositionId");
            CreateIndex("DMS.AdminUserRoles", new[] { "UserId", "RoleId", "StartDate", "PositionExecutorId" }, unique: true, name: "IX_UserRoleStartDate");
            CreateIndex("DMS.DictionaryAgentUsers", "UserId", unique: true);
            CreateIndex("DMS.DictionaryAgentAccounts", "AgentBankId");
            AddForeignKey("DMS.AdminUserRoles", "PositionId", "DMS.DictionaryPositions", "Id");
        }
    }
}
