namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SigningType : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "DMS.DictionarySigningTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 400),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true);
            
            AddColumn("DMS.EncryptionCertificates", "IsRememberPassword", c => c.Boolean(nullable: false));
            AddColumn("DMS.DocumentSubscriptions", "SigningTypeId", c => c.Int());
            CreateIndex("DMS.DocumentSubscriptions", "SigningTypeId");
            AddForeignKey("DMS.DocumentSubscriptions", "SigningTypeId", "DMS.DictionarySigningTypes", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("DMS.DocumentSubscriptions", "SigningTypeId", "DMS.DictionarySigningTypes");
            DropIndex("DMS.DictionarySigningTypes", new[] { "Name" });
            DropIndex("DMS.DocumentSubscriptions", new[] { "SigningTypeId" });
            DropColumn("DMS.DocumentSubscriptions", "SigningTypeId");
            DropColumn("DMS.EncryptionCertificates", "IsRememberPassword");
            DropTable("DMS.DictionarySigningTypes");
        }
    }
}
