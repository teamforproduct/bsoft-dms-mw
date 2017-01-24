namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class StageTypes : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "DMS.DictionaryStageTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(maxLength: 2000),
                        Name = c.String(maxLength: 2000),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true);
            
            AddColumn("DMS.DocumentSendLists", "StageTypeId", c => c.Int());
            AddColumn("DMS.TemplateDocumentSendLists", "StageTypeId", c => c.Int());
            CreateIndex("DMS.DocumentSendLists", "StageTypeId");
            CreateIndex("DMS.TemplateDocumentSendLists", "StageTypeId");
            AddForeignKey("DMS.DocumentSendLists", "StageTypeId", "DMS.DictionaryStageTypes", "Id");
            AddForeignKey("DMS.TemplateDocumentSendLists", "StageTypeId", "DMS.DictionaryStageTypes", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("DMS.TemplateDocumentSendLists", "StageTypeId", "DMS.DictionaryStageTypes");
            DropForeignKey("DMS.DocumentSendLists", "StageTypeId", "DMS.DictionaryStageTypes");
            DropIndex("DMS.TemplateDocumentSendLists", new[] { "StageTypeId" });
            DropIndex("DMS.DictionaryStageTypes", new[] { "Name" });
            DropIndex("DMS.DocumentSendLists", new[] { "StageTypeId" });
            DropColumn("DMS.TemplateDocumentSendLists", "StageTypeId");
            DropColumn("DMS.DocumentSendLists", "StageTypeId");
            DropTable("DMS.DictionaryStageTypes");
        }
    }
}
