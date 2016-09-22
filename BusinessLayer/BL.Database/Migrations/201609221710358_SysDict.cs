namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SysDict : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "DMS.SystemFormats",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(maxLength: 400),
                        Name = c.String(maxLength: 2000),
                        Description = c.String(maxLength: 2000),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Code, unique: true);
            
            CreateTable(
                "DMS.SystemFormulas",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(maxLength: 400),
                        Name = c.String(maxLength: 2000),
                        Description = c.String(maxLength: 2000),
                        Example = c.String(maxLength: 2000),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Code, unique: true);
            
            CreateTable(
                "DMS.SystemPatterns",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(maxLength: 400),
                        Name = c.String(maxLength: 2000),
                        Description = c.String(maxLength: 2000),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Code, unique: true);
            
        }
        
        public override void Down()
        {
            DropIndex("DMS.SystemPatterns", new[] { "Code" });
            DropIndex("DMS.SystemFormulas", new[] { "Code" });
            DropIndex("DMS.SystemFormats", new[] { "Code" });
            DropTable("DMS.SystemPatterns");
            DropTable("DMS.SystemFormulas");
            DropTable("DMS.SystemFormats");
        }
    }
}
