namespace DMS_WebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserFingerPrintIndexOff : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.AspNetUserFingerprints", "IX_UserName");
        }
        
        public override void Down()
        {
            CreateIndex("dbo.AspNetUserFingerprints", new[] { "UserId", "Name" }, unique: true, name: "IX_UserName");
        }
    }
}
