namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AdminPositionRoles : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.AdminRoles", "AccessLevelId", "dbo.AdminAccessLevels");
            DropForeignKey("dbo.AdminRoles", "PositionId", "dbo.DictionaryPositions");
            DropIndex("dbo.AdminRoles", new[] { "PositionId" });
            DropIndex("dbo.AdminRoles", new[] { "AccessLevelId" });
            CreateTable(
                "dbo.AdminPositionRoles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RoleId = c.Int(nullable: false),
                        PositionId = c.Int(nullable: false),
                        AccessLevelId = c.Int(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AdminAccessLevels", t => t.AccessLevelId)
                .ForeignKey("dbo.DictionaryPositions", t => t.PositionId)
                .ForeignKey("dbo.AdminRoles", t => t.RoleId)
                .Index(t => t.RoleId)
                .Index(t => t.PositionId)
                .Index(t => t.AccessLevelId);
            
            AddColumn("dbo.DictionaryAgents", "IsCompany", c => c.Boolean(nullable: false));
            AddColumn("dbo.DictionaryAgents", "IsUser", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AdminPositionRoles", "RoleId", "dbo.AdminRoles");
            DropForeignKey("dbo.AdminPositionRoles", "PositionId", "dbo.DictionaryPositions");
            DropForeignKey("dbo.AdminPositionRoles", "AccessLevelId", "dbo.AdminAccessLevels");
            DropIndex("dbo.AdminPositionRoles", new[] { "AccessLevelId" });
            DropIndex("dbo.AdminPositionRoles", new[] { "PositionId" });
            DropIndex("dbo.AdminPositionRoles", new[] { "RoleId" });
            DropColumn("dbo.DictionaryAgents", "IsUser");
            DropColumn("dbo.DictionaryAgents", "IsCompany");
            DropTable("dbo.AdminPositionRoles");
            CreateIndex("dbo.AdminRoles", "AccessLevelId");
            CreateIndex("dbo.AdminRoles", "PositionId");
            AddForeignKey("dbo.AdminRoles", "PositionId", "dbo.DictionaryPositions", "Id");
            AddForeignKey("dbo.AdminRoles", "AccessLevelId", "dbo.AdminAccessLevels", "Id");
        }
    }
}
