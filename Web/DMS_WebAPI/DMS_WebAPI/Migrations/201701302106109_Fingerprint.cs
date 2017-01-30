namespace DMS_WebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Fingerprint : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SystemControlQuestions",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Name = c.String(maxLength: 200),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "IX_FileName");
            
            CreateTable(
                "dbo.AspNetUserFingerprints",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(maxLength: 128),
                        Name = c.String(maxLength: 2000),
                        Fingerprint = c.String(maxLength: 2000),
                        Browser = c.String(maxLength: 2000),
                        Platform = c.String(maxLength: 2000),
                        IsActive = c.Boolean(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => new { t.UserId, t.Fingerprint }, unique: true, name: "IX_UserFingerprint")
                .Index(t => new { t.UserId, t.Name }, unique: true, name: "IX_UserName");
            
            AddColumn("dbo.AspNetUsers", "IsFingerprintEnabled", c => c.Boolean(nullable: false));
            AddColumn("dbo.AspNetUsers", "ControlQuestionId", c => c.Int());
            AddColumn("dbo.AspNetUsers", "ControlAnswer", c => c.String());
            CreateIndex("dbo.AspNetUsers", "ControlQuestionId");
            AddForeignKey("dbo.AspNetUsers", "ControlQuestionId", "dbo.SystemControlQuestions", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserFingerprints", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUsers", "ControlQuestionId", "dbo.SystemControlQuestions");
            DropIndex("dbo.AspNetUserFingerprints", "IX_UserName");
            DropIndex("dbo.AspNetUserFingerprints", "IX_UserFingerprint");
            DropIndex("dbo.SystemControlQuestions", "IX_FileName");
            DropIndex("dbo.AspNetUsers", new[] { "ControlQuestionId" });
            DropColumn("dbo.AspNetUsers", "ControlAnswer");
            DropColumn("dbo.AspNetUsers", "ControlQuestionId");
            DropColumn("dbo.AspNetUsers", "IsFingerprintEnabled");
            DropTable("dbo.AspNetUserFingerprints");
            DropTable("dbo.SystemControlQuestions");
        }
    }
}
