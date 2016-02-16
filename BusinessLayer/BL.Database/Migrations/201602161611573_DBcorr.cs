namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DBcorr : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AdminSubordinations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PositionId = c.Int(nullable: false),
                        AddresseePositionId = c.Int(nullable: false),
                        SubordinationTypeId = c.Int(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DictionaryPositions", t => t.AddresseePositionId)
                .ForeignKey("dbo.DictionaryPositions", t => t.PositionId)
                .ForeignKey("dbo.DictionarySubordinationTypes", t => t.SubordinationTypeId)
                .Index(t => t.PositionId)
                .Index(t => t.AddresseePositionId)
                .Index(t => t.SubordinationTypeId);
            
            AddColumn("dbo.SystemActions", "GrantId", c => c.Int());
            AddColumn("dbo.DocumentEvents", "ReadDate", c => c.DateTime());
            CreateIndex("dbo.SystemActions", "GrantId");
            AddForeignKey("dbo.SystemActions", "GrantId", "dbo.SystemActions", "Id");
            DropColumn("dbo.TemplateDocuments", "SenderNumber");
            DropColumn("dbo.TemplateDocuments", "SenderDate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TemplateDocuments", "SenderDate", c => c.DateTime());
            AddColumn("dbo.TemplateDocuments", "SenderNumber", c => c.String());
            DropForeignKey("dbo.AdminSubordinations", "SubordinationTypeId", "dbo.DictionarySubordinationTypes");
            DropForeignKey("dbo.AdminSubordinations", "PositionId", "dbo.DictionaryPositions");
            DropForeignKey("dbo.AdminSubordinations", "AddresseePositionId", "dbo.DictionaryPositions");
            DropForeignKey("dbo.SystemActions", "GrantId", "dbo.SystemActions");
            DropIndex("dbo.AdminSubordinations", new[] { "SubordinationTypeId" });
            DropIndex("dbo.AdminSubordinations", new[] { "AddresseePositionId" });
            DropIndex("dbo.AdminSubordinations", new[] { "PositionId" });
            DropIndex("dbo.SystemActions", new[] { "GrantId" });
            DropColumn("dbo.DocumentEvents", "ReadDate");
            DropColumn("dbo.SystemActions", "GrantId");
            DropTable("dbo.AdminSubordinations");
        }
    }
}
