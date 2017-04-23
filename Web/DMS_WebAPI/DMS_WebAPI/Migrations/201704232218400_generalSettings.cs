namespace DMS_WebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class generalSettings : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AspNetClientRequests",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClientCode = c.String(maxLength: 200),
                        ClientName = c.String(maxLength: 2000),
                        Language = c.String(maxLength: 10),
                        Email = c.String(maxLength: 256),
                        FirstName = c.String(maxLength: 256),
                        LastName = c.String(maxLength: 256),
                        MiddleName = c.String(maxLength: 256),
                        PhoneNumber = c.String(maxLength: 2000),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.ClientCode, unique: true, name: "IX_Code");
            
            CreateTable(
                "dbo.SystemSettings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Key = c.String(maxLength: 400),
                        Name = c.String(maxLength: 400),
                        Description = c.String(maxLength: 2000),
                        Value = c.String(maxLength: 2000),
                        ValueTypeId = c.Int(nullable: false),
                        Order = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.SystemValueTypes", t => t.ValueTypeId, cascadeDelete: true)
                .Index(t => t.Key, unique: true)
                .Index(t => t.ValueTypeId);
            
            CreateTable(
                "dbo.SystemValueTypes",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Code = c.String(maxLength: 400),
                        Description = c.String(maxLength: 2000),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Code, unique: true);
            
            AlterColumn("dbo.AspNetClients", "Code", c => c.String(maxLength: 256));
            CreateIndex("dbo.AspNetClients", "Code", unique: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SystemSettings", "ValueTypeId", "dbo.SystemValueTypes");
            DropIndex("dbo.SystemValueTypes", new[] { "Code" });
            DropIndex("dbo.SystemSettings", new[] { "ValueTypeId" });
            DropIndex("dbo.SystemSettings", new[] { "Key" });
            DropIndex("dbo.AspNetClientRequests", "IX_Code");
            DropIndex("dbo.AspNetClients", new[] { "Code" });
            AlterColumn("dbo.AspNetClients", "Code", c => c.String(maxLength: 2000));
            DropTable("dbo.SystemValueTypes");
            DropTable("dbo.SystemSettings");
            DropTable("dbo.AspNetClientRequests");
        }
    }
}
