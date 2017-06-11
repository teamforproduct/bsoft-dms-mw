namespace DMS_WebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveLangLabels : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.SystemControlQuestions", "IX_FileName");
            DropIndex("dbo.SystemValueTypes", new[] { "Code" });
            AddColumn("dbo.SystemControlQuestions", "Code", c => c.String(maxLength: 400));
            DropColumn("dbo.SystemControlQuestions", "Name");
            DropColumn("dbo.SystemSettings", "Name");
            DropColumn("dbo.SystemSettings", "Description");
            DropColumn("dbo.SystemValueTypes", "Description");
        }
        
        public override void Down()
        {
            AddColumn("dbo.SystemValueTypes", "Description", c => c.String(maxLength: 2000));
            AddColumn("dbo.SystemSettings", "Description", c => c.String(maxLength: 2000));
            AddColumn("dbo.SystemSettings", "Name", c => c.String(maxLength: 400));
            AddColumn("dbo.SystemControlQuestions", "Name", c => c.String(maxLength: 200));
            DropColumn("dbo.SystemControlQuestions", "Code");
            CreateIndex("dbo.SystemValueTypes", "Code", unique: true);
            CreateIndex("dbo.SystemControlQuestions", "Name", unique: true, name: "IX_FileName");
        }
    }
}
