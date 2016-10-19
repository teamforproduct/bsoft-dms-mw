namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changeLogs : DbMigration
    {
        public override void Up()
        {
            AddColumn("DMS.SystemLogs", "ObjectId", c => c.Int());
            AddColumn("DMS.SystemLogs", "ActionId", c => c.Int());
            AddColumn("DMS.SystemLogs", "RecordId", c => c.Int());
            CreateIndex("DMS.SystemLogs", "ObjectId");
            CreateIndex("DMS.SystemLogs", "ActionId");
            AddForeignKey("DMS.SystemLogs", "ActionId", "DMS.SystemActions", "Id");
            AddForeignKey("DMS.SystemLogs", "ObjectId", "DMS.SystemObjects", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("DMS.SystemLogs", "ObjectId", "DMS.SystemObjects");
            DropForeignKey("DMS.SystemLogs", "ActionId", "DMS.SystemActions");
            DropIndex("DMS.SystemLogs", new[] { "ActionId" });
            DropIndex("DMS.SystemLogs", new[] { "ObjectId" });
            DropColumn("DMS.SystemLogs", "RecordId");
            DropColumn("DMS.SystemLogs", "ActionId");
            DropColumn("DMS.SystemLogs", "ObjectId");
        }
    }
}
