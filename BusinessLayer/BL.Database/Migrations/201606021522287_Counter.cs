namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Counter : DbMigration
    {
        public override void Up()
        {
            CreateIndex("DMS.Documents", "LinkId");
            AddForeignKey("DMS.Documents", "LinkId", "DMS.Documents", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("DMS.Documents", "LinkId", "DMS.Documents");
            DropIndex("DMS.Documents", new[] { "LinkId" });
        }
    }
}
