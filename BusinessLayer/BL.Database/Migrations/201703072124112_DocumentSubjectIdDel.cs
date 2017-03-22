namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DocumentSubjectIdDel : DbMigration
    {
        public override void Up()
        {
            DropColumn("DMS.Documents", "DocumentSubjectId");
            DropColumn("DMS.TemplateDocuments", "DocumentSubjectId");
        }
        
        public override void Down()
        {
            AddColumn("DMS.TemplateDocuments", "DocumentSubjectId", c => c.Int());
            AddColumn("DMS.Documents", "DocumentSubjectId", c => c.Int());
        }
    }
}
