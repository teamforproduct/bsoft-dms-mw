namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EncryptionCer : DbMigration
    {
        public override void Up()
        {
            DropIndex("DMS.DictionaryStandartSendLists", "IX_Name");
            DropIndex("DMS.DictionaryStandartSendLists", "IX_PositionName");
            CreateTable(
                "DMS.EncryptionCertificates",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Certificate = c.Binary(),
                        Extension = c.String(),
                        CreateDate = c.DateTime(nullable: false),
                        ValidFromDate = c.DateTime(),
                        ValidToDate = c.DateTime(),
                        IsPublic = c.Boolean(nullable: false),
                        IsPrivate = c.Boolean(nullable: false),
                        AgentId = c.Int(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.DictionaryAgents", t => t.AgentId)
                .Index(t => t.AgentId);
            
            AlterColumn("DMS.DictionaryStandartSendLists", "Name", c => c.String(maxLength: 400));
            CreateIndex("DMS.DictionaryStandartSendLists", new[] { "PositionId", "Name", "ClientId" }, unique: true, name: "IX_PositionName");
        }
        
        public override void Down()
        {
            DropForeignKey("DMS.EncryptionCertificates", "AgentId", "DMS.DictionaryAgents");
            DropIndex("DMS.DictionaryStandartSendLists", "IX_PositionName");
            DropIndex("DMS.EncryptionCertificates", new[] { "AgentId" });
            AlterColumn("DMS.DictionaryStandartSendLists", "Name", c => c.String(maxLength: 2000));
            DropTable("DMS.EncryptionCertificates");
            CreateIndex("DMS.DictionaryStandartSendLists", new[] { "PositionId", "Name" }, unique: true, name: "IX_PositionName");
            CreateIndex("DMS.DictionaryStandartSendLists", "ClientId", unique: true, name: "IX_Name");
        }
    }
}
