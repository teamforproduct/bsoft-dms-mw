namespace DMS_WebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFileNameToLanguages2 : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.AdminLanguages", new[] { "FileName" });
            CreateIndex("dbo.AdminLanguages", "FileName", unique: true);
        }
        
        public override void Down()
        {
            DropIndex("dbo.AdminLanguages", new[] { "FileName" });
            CreateIndex("dbo.AdminLanguages", "FileName");
        }
    }
}
