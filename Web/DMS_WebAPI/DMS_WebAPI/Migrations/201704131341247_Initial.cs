namespace DMS_WebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AdminLanguages",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Code = c.String(maxLength: 200),
                        Name = c.String(maxLength: 200),
                        FileName = c.String(maxLength: 200),
                        IsDefault = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.FileName, unique: true);
            
            CreateTable(
                "dbo.AdminServers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Address = c.String(maxLength: 2000),
                        Name = c.String(maxLength: 2000),
                        ServerType = c.String(maxLength: 2000),
                        DefaultDatabase = c.String(maxLength: 2000),
                        IntegrateSecurity = c.Boolean(nullable: false),
                        UserName = c.String(maxLength: 2000),
                        UserPassword = c.String(maxLength: 2000),
                        ConnectionString = c.String(maxLength: 2000),
                        DefaultSchema = c.String(maxLength: 2000),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetClientServers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClientId = c.Int(nullable: false),
                        ServerId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetClients", t => t.ClientId, cascadeDelete: true)
                .ForeignKey("dbo.AdminServers", t => t.ServerId, cascadeDelete: true)
                .Index(t => t.ClientId)
                .Index(t => t.ServerId);
            
            CreateTable(
                "dbo.AspNetClients",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 2000),
                        Code = c.String(maxLength: 2000),
                        VerificationCode = c.String(maxLength: 2000),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetClientLicences",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClientId = c.Int(nullable: false),
                        LicenceId = c.Int(nullable: false),
                        FirstStart = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        LicenceKey = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetClients", t => t.ClientId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetLicences", t => t.LicenceId, cascadeDelete: true)
                .Index(t => t.ClientId)
                .Index(t => t.LicenceId);
            
            CreateTable(
                "dbo.AspNetLicences",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 2000),
                        Description = c.String(maxLength: 2000),
                        NamedNumberOfConnections = c.Int(),
                        ConcurenteNumberOfConnections = c.Int(),
                        DurationDay = c.Int(),
                        Functionals = c.String(maxLength: 2000),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetUserServers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(maxLength: 128),
                        ServerId = c.Int(nullable: false),
                        ClientId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetClients", t => t.ClientId, cascadeDelete: true)
                .ForeignKey("dbo.AdminServers", t => t.ServerId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.ServerId)
                .Index(t => t.ClientId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        IsChangePasswordRequired = c.Boolean(nullable: false),
                        IsEmailConfirmRequired = c.Boolean(nullable: false),
                        IsLockout = c.Boolean(nullable: false),
                        IsFingerprintEnabled = c.Boolean(nullable: false),
                        ControlQuestionId = c.Int(),
                        ControlAnswer = c.String(),
                        CreateDate = c.DateTime(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.SystemControlQuestions", t => t.ControlQuestionId)
                .Index(t => t.ControlQuestionId)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
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
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
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
                .Index(t => new { t.UserId, t.Fingerprint }, unique: true, name: "IX_UserFingerprint");
            
            CreateTable(
                "dbo.AspNetUserClients",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(maxLength: 128),
                        ClientId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetClients", t => t.ClientId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.ClientId);
            
            CreateTable(
                "dbo.AspNetUserContexts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Token = c.String(maxLength: 550),
                        UserId = c.String(maxLength: 128),
                        UserName = c.String(maxLength: 256),
                        ClientId = c.Int(nullable: false),
                        CurrentPositionsIdList = c.String(maxLength: 400),
                        DatabaseId = c.Int(nullable: false),
                        IsChangePasswordRequired = c.Boolean(nullable: false),
                        LoginLogId = c.Int(),
                        LoginLogInfo = c.String(maxLength: 2000),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.AspNetUserClients", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClients", "ClientId", "dbo.AspNetClients");
            DropForeignKey("dbo.AspNetClientServers", "ServerId", "dbo.AdminServers");
            DropForeignKey("dbo.AspNetUserServers", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserFingerprints", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUsers", "ControlQuestionId", "dbo.SystemControlQuestions");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserServers", "ServerId", "dbo.AdminServers");
            DropForeignKey("dbo.AspNetUserServers", "ClientId", "dbo.AspNetClients");
            DropForeignKey("dbo.AspNetClientLicences", "LicenceId", "dbo.AspNetLicences");
            DropForeignKey("dbo.AspNetClientLicences", "ClientId", "dbo.AspNetClients");
            DropForeignKey("dbo.AspNetClientServers", "ClientId", "dbo.AspNetClients");
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.AspNetUserClients", new[] { "ClientId" });
            DropIndex("dbo.AspNetUserClients", new[] { "UserId" });
            DropIndex("dbo.AspNetUserFingerprints", "IX_UserFingerprint");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.SystemControlQuestions", "IX_FileName");
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUsers", new[] { "ControlQuestionId" });
            DropIndex("dbo.AspNetUserServers", new[] { "ClientId" });
            DropIndex("dbo.AspNetUserServers", new[] { "ServerId" });
            DropIndex("dbo.AspNetUserServers", new[] { "UserId" });
            DropIndex("dbo.AspNetClientLicences", new[] { "LicenceId" });
            DropIndex("dbo.AspNetClientLicences", new[] { "ClientId" });
            DropIndex("dbo.AspNetClientServers", new[] { "ServerId" });
            DropIndex("dbo.AspNetClientServers", new[] { "ClientId" });
            DropIndex("dbo.AdminLanguages", new[] { "FileName" });
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.AspNetUserContexts");
            DropTable("dbo.AspNetUserClients");
            DropTable("dbo.AspNetUserFingerprints");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.SystemControlQuestions");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.AspNetUserServers");
            DropTable("dbo.AspNetLicences");
            DropTable("dbo.AspNetClientLicences");
            DropTable("dbo.AspNetClients");
            DropTable("dbo.AspNetClientServers");
            DropTable("dbo.AdminServers");
            DropTable("dbo.AdminLanguages");
        }
    }
}
