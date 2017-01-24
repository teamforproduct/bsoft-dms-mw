namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Temp : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "DMS.SystemPermissions", newName: "SystemPermissions2");
            RenameTable(name: "DMS.SystemAccessTypes", newName: "SystemAccessTypes2");
            RenameTable(name: "DMS.SystemFeatures", newName: "SystemFeatures2");
            RenameTable(name: "DMS.SystemModules", newName: "SystemModules2");
            DropIndex("DMS.SystemModules123", new[] { "Code" });
            DropTable("DMS.SystemModules123");
        }
        
        public override void Down()
        {
            CreateTable(
                "DMS.SystemModules123",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Code = c.String(maxLength: 400),
                        Name = c.String(maxLength: 2000),
                        Order = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateIndex("DMS.SystemModules123", "Code", unique: true);
            RenameTable(name: "DMS.SystemModules2", newName: "SystemModules");
            RenameTable(name: "DMS.SystemFeatures2", newName: "SystemFeatures");
            RenameTable(name: "DMS.SystemAccessTypes2", newName: "SystemAccessTypes");
            RenameTable(name: "DMS.SystemPermissions2", newName: "SystemPermissions");
        }
    }
}
