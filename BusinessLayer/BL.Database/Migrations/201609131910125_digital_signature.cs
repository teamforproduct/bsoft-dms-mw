namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class digital_signature : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("DMS.EncryptionCertificates", "TypeId", "DMS.EncryptionCertificateTypes");
            DropIndex("DMS.EncryptionCertificates", new[] { "TypeId" });
            CreateTable(
                "DMS.DictionaryFileTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 400),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true);
            
            AddColumn("DMS.EncryptionCertificates", "Thumbprint", c => c.String());
            AddColumn("DMS.EncryptionCertificates", "NotBefore", c => c.DateTime());
            AddColumn("DMS.EncryptionCertificates", "NotAfter", c => c.DateTime());
            AddColumn("DMS.DocumentSubscriptions", "InternalSign", c => c.String());
            AddColumn("DMS.DocumentSubscriptions", "CertificateSign", c => c.String());
            AddColumn("DMS.DocumentSubscriptions", "CertificateId", c => c.Int());
            AddColumn("DMS.DocumentSubscriptions", "CertificateSignCreateDate", c => c.DateTime());
            AddColumn("DMS.DocumentSubscriptions", "CertificatePositionId", c => c.Int());
            AddColumn("DMS.DocumentSubscriptions", "CertificatePositionExecutorAgentId", c => c.Int());
            AddColumn("DMS.DocumentFiles", "TypeId", c => c.Int());
            AddColumn("DMS.TemplateDocumentFiles", "TypeId", c => c.Int());
            CreateIndex("DMS.DocumentSubscriptions", "CertificateId");
            CreateIndex("DMS.DocumentSubscriptions", "CertificatePositionId");
            CreateIndex("DMS.DocumentSubscriptions", "CertificatePositionExecutorAgentId");
            CreateIndex("DMS.DocumentFiles", "TypeId");
            CreateIndex("DMS.TemplateDocumentFiles", "TypeId");
            AddForeignKey("DMS.DocumentSubscriptions", "CertificateId", "DMS.EncryptionCertificates", "Id");
            AddForeignKey("DMS.DocumentSubscriptions", "CertificatePositionId", "DMS.DictionaryPositions", "Id");
            AddForeignKey("DMS.DocumentSubscriptions", "CertificatePositionExecutorAgentId", "DMS.DictionaryAgents", "Id");
            AddForeignKey("DMS.DocumentFiles", "TypeId", "DMS.DictionaryFileTypes", "Id");
            AddForeignKey("DMS.TemplateDocumentFiles", "TypeId", "DMS.DictionaryFileTypes", "Id");
            DropColumn("DMS.EncryptionCertificates", "Extension");
            DropColumn("DMS.EncryptionCertificates", "ValidFromDate");
            DropColumn("DMS.EncryptionCertificates", "ValidToDate");
            DropColumn("DMS.EncryptionCertificates", "IsPublic");
            DropColumn("DMS.EncryptionCertificates", "IsPrivate");
            DropColumn("DMS.EncryptionCertificates", "TypeId");
            DropColumn("DMS.DocumentFiles", "IsAdditional");
            DropColumn("DMS.TemplateDocumentFiles", "IsAdditional");
            DropTable("DMS.EncryptionCertificateTypes");
        }
        
        public override void Down()
        {
            CreateTable(
                "DMS.EncryptionCertificateTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(maxLength: 2000),
                        Name = c.String(maxLength: 2000),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("DMS.TemplateDocumentFiles", "IsAdditional", c => c.Boolean(nullable: false));
            AddColumn("DMS.DocumentFiles", "IsAdditional", c => c.Boolean(nullable: false));
            AddColumn("DMS.EncryptionCertificates", "TypeId", c => c.Int(nullable: false));
            AddColumn("DMS.EncryptionCertificates", "IsPrivate", c => c.Boolean(nullable: false));
            AddColumn("DMS.EncryptionCertificates", "IsPublic", c => c.Boolean(nullable: false));
            AddColumn("DMS.EncryptionCertificates", "ValidToDate", c => c.DateTime());
            AddColumn("DMS.EncryptionCertificates", "ValidFromDate", c => c.DateTime());
            AddColumn("DMS.EncryptionCertificates", "Extension", c => c.String(maxLength: 400));
            DropForeignKey("DMS.TemplateDocumentFiles", "TypeId", "DMS.DictionaryFileTypes");
            DropForeignKey("DMS.DocumentFiles", "TypeId", "DMS.DictionaryFileTypes");
            DropForeignKey("DMS.DocumentSubscriptions", "CertificatePositionExecutorAgentId", "DMS.DictionaryAgents");
            DropForeignKey("DMS.DocumentSubscriptions", "CertificatePositionId", "DMS.DictionaryPositions");
            DropForeignKey("DMS.DocumentSubscriptions", "CertificateId", "DMS.EncryptionCertificates");
            DropIndex("DMS.TemplateDocumentFiles", new[] { "TypeId" });
            DropIndex("DMS.DictionaryFileTypes", new[] { "Name" });
            DropIndex("DMS.DocumentFiles", new[] { "TypeId" });
            DropIndex("DMS.DocumentSubscriptions", new[] { "CertificatePositionExecutorAgentId" });
            DropIndex("DMS.DocumentSubscriptions", new[] { "CertificatePositionId" });
            DropIndex("DMS.DocumentSubscriptions", new[] { "CertificateId" });
            DropColumn("DMS.TemplateDocumentFiles", "TypeId");
            DropColumn("DMS.DocumentFiles", "TypeId");
            DropColumn("DMS.DocumentSubscriptions", "CertificatePositionExecutorAgentId");
            DropColumn("DMS.DocumentSubscriptions", "CertificatePositionId");
            DropColumn("DMS.DocumentSubscriptions", "CertificateSignCreateDate");
            DropColumn("DMS.DocumentSubscriptions", "CertificateId");
            DropColumn("DMS.DocumentSubscriptions", "CertificateSign");
            DropColumn("DMS.DocumentSubscriptions", "InternalSign");
            DropColumn("DMS.EncryptionCertificates", "NotAfter");
            DropColumn("DMS.EncryptionCertificates", "NotBefore");
            DropColumn("DMS.EncryptionCertificates", "Thumbprint");
            DropTable("DMS.DictionaryFileTypes");
            CreateIndex("DMS.EncryptionCertificates", "TypeId");
            AddForeignKey("DMS.EncryptionCertificates", "TypeId", "DMS.EncryptionCertificateTypes", "Id");
        }
    }
}
