namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Properties : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Properties",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(),
                        Description = c.String(),
                        Label = c.String(),
                        Hint = c.String(),
                        ValueTypeId = c.Int(),
                        OutFormat = c.String(),
                        InputFormat = c.String(),
                        SelectAPI = c.String(),
                        SelectFilter = c.String(),
                        SelectFieldCode = c.String(),
                        SelectDescriptionFieldCode = c.String(),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.SystemValueTypes", t => t.ValueTypeId)
                .Index(t => t.ValueTypeId);
            
            CreateTable(
                "dbo.PropertyLinks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PropertyId = c.Int(nullable: false),
                        ObjectId = c.Int(nullable: false),
                        Filers = c.String(),
                        IsMandatory = c.Boolean(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.SystemObjects", t => t.ObjectId)
                .ForeignKey("dbo.Properties", t => t.PropertyId)
                .Index(t => t.PropertyId)
                .Index(t => t.ObjectId);
            
            CreateTable(
                "dbo.PropertyValues",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PropertyLinkId = c.Int(nullable: false),
                        RecordId = c.Int(nullable: false),
                        ValueString = c.String(),
                        ValueDate = c.DateTime(),
                        ValueNumeric = c.Double(),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.PropertyLinks", t => t.PropertyLinkId)
                .Index(t => t.PropertyLinkId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PropertyValues", "PropertyLinkId", "dbo.PropertyLinks");
            DropForeignKey("dbo.PropertyLinks", "PropertyId", "dbo.Properties");
            DropForeignKey("dbo.PropertyLinks", "ObjectId", "dbo.SystemObjects");
            DropForeignKey("dbo.Properties", "ValueTypeId", "dbo.SystemValueTypes");
            DropIndex("dbo.PropertyValues", new[] { "PropertyLinkId" });
            DropIndex("dbo.PropertyLinks", new[] { "ObjectId" });
            DropIndex("dbo.PropertyLinks", new[] { "PropertyId" });
            DropIndex("dbo.Properties", new[] { "ValueTypeId" });
            DropTable("dbo.PropertyValues");
            DropTable("dbo.PropertyLinks");
            DropTable("dbo.Properties");
        }
    }
}
