namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SystemUIElements : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SystemUIElements",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ActionId = c.Int(nullable: false),
                        Code = c.String(),
                        TypeCode = c.String(),
                        Description = c.String(),
                        Lable = c.String(),
                        Hint = c.String(),
                        ValueTypeId = c.Int(nullable: false),
                        IsMandatory = c.Boolean(nullable: false),
                        IsReadOnly = c.Boolean(nullable: false),
                        IsVisible = c.Boolean(nullable: false),
                        SelectAPI = c.String(),
                        SelectFiler = c.String(),
                        SelectFieldCode = c.String(),
                        SelectDescriptionFieldCode = c.String(),
                        ValueFieldCode = c.String(),
                        ValueDescriptionFieldCode = c.String(),
                        Format = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.SystemActions", t => t.ActionId)
                .ForeignKey("dbo.SystemValueTypes", t => t.ValueTypeId)
                .Index(t => t.ActionId)
                .Index(t => t.ValueTypeId);
            
            AlterColumn("dbo.TemplateDocuments", "IsHard", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SystemUIElements", "ValueTypeId", "dbo.SystemValueTypes");
            DropForeignKey("dbo.SystemUIElements", "ActionId", "dbo.SystemActions");
            DropIndex("dbo.SystemUIElements", new[] { "ValueTypeId" });
            DropIndex("dbo.SystemUIElements", new[] { "ActionId" });
            AlterColumn("dbo.TemplateDocuments", "IsHard", c => c.Int(nullable: false));
            DropTable("dbo.SystemUIElements");
        }
    }
}
