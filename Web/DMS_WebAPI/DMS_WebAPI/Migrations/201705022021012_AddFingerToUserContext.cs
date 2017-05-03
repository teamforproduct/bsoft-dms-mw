namespace DMS_WebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFingerToUserContext : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.AspNetUserFingerprints", "IX_UserFingerprint");
            AddColumn("dbo.AspNetUserContexts", "Fingerprint", c => c.String(maxLength: 36));
            AlterColumn("dbo.AspNetUserFingerprints", "Name", c => c.String(maxLength: 256));
            AlterColumn("dbo.AspNetUserFingerprints", "Fingerprint", c => c.String(maxLength: 36));
            AlterColumn("dbo.AspNetUserFingerprints", "Browser", c => c.String(maxLength: 40));
            AlterColumn("dbo.AspNetUserFingerprints", "Platform", c => c.String(maxLength: 40));
            CreateIndex("dbo.AspNetUserFingerprints", new[] { "UserId", "Fingerprint" }, unique: true, name: "IX_UserFingerprint");
        }
        
        public override void Down()
        {
            DropIndex("dbo.AspNetUserFingerprints", "IX_UserFingerprint");
            AlterColumn("dbo.AspNetUserFingerprints", "Platform", c => c.String(maxLength: 2000));
            AlterColumn("dbo.AspNetUserFingerprints", "Browser", c => c.String(maxLength: 2000));
            AlterColumn("dbo.AspNetUserFingerprints", "Fingerprint", c => c.String(maxLength: 2000));
            AlterColumn("dbo.AspNetUserFingerprints", "Name", c => c.String(maxLength: 2000));
            DropColumn("dbo.AspNetUserContexts", "Fingerprint");
            CreateIndex("dbo.AspNetUserFingerprints", new[] { "UserId", "Fingerprint" }, unique: true, name: "IX_UserFingerprint");
        }
    }
}
