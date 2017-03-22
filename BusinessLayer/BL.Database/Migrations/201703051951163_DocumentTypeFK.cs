namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DocumentTypeFK : DbMigration
    {
        public override void Up()
        {
            CreateIndex("DMS.Documents", "DocumentDirectionId");
            CreateIndex("DMS.Documents", "DocumentTypeId");
            AddForeignKey("DMS.Documents", "DocumentDirectionId", "DMS.DictionaryDocumentDirections", "Id");
            AddForeignKey("DMS.Documents", "DocumentTypeId", "DMS.DictionaryDocumentTypes", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("DMS.Documents", "DocumentTypeId", "DMS.DictionaryDocumentTypes");
            DropForeignKey("DMS.Documents", "DocumentDirectionId", "DMS.DictionaryDocumentDirections");
            DropIndex("DMS.Documents", new[] { "DocumentTypeId" });
            DropIndex("DMS.Documents", new[] { "DocumentDirectionId" });
        }
    }
}
