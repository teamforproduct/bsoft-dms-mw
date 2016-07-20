namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveFKLinks : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("DMS.Documents", "LinkId", "DMS.Documents");
            DropIndex("DMS.Documents", new[] { "LinkId" });
        }
        
        public override void Down()
        {
            CreateIndex("DMS.Documents", "LinkId");
            AddForeignKey("DMS.Documents", "LinkId", "DMS.Documents", "Id");
        }
    }
}
