namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SystemActionsCategoryId : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("DMS.SystemActions", "SystemActions_Id", "DMS.SystemActions");
            DropIndex("DMS.SystemActions", new[] { "SystemActions_Id" });
            AddColumn("DMS.SystemActions", "CategoryId", c => c.Int());
            DropColumn("DMS.SystemActions", "SystemActions_Id");
        }
        
        public override void Down()
        {
            AddColumn("DMS.SystemActions", "SystemActions_Id", c => c.Int());
            DropColumn("DMS.SystemActions", "CategoryId");
            CreateIndex("DMS.SystemActions", "SystemActions_Id");
            AddForeignKey("DMS.SystemActions", "SystemActions_Id", "DMS.SystemActions", "Id");
        }
    }
}
