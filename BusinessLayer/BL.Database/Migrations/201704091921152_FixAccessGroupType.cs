namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixAccessGroupType : DbMigration
    {
        public override void Up()
        {
            AddColumn("DMS.DocumentSendListAccessGroups", "AccessGroupTypeId", c => c.Int(nullable: false));
            AddColumn("DMS.DocumentEventAccessGroups", "AccessGroupTypeId", c => c.Int(nullable: false));
            AddColumn("DMS.TemplateDocumentSendListAccessGroups", "AccessGroupTypeId", c => c.Int(nullable: false));
            DropColumn("DMS.DocumentSendListAccessGroups", "AccessGroupsTypeId");
            DropColumn("DMS.DocumentEventAccessGroups", "AccessGroupsTypeId");
            DropColumn("DMS.TemplateDocumentSendListAccessGroups", "AccessGroupsTypeId");
        }
        
        public override void Down()
        {
            AddColumn("DMS.TemplateDocumentSendListAccessGroups", "AccessGroupsTypeId", c => c.Int(nullable: false));
            AddColumn("DMS.DocumentEventAccessGroups", "AccessGroupsTypeId", c => c.Int(nullable: false));
            AddColumn("DMS.DocumentSendListAccessGroups", "AccessGroupsTypeId", c => c.Int(nullable: false));
            DropColumn("DMS.TemplateDocumentSendListAccessGroups", "AccessGroupTypeId");
            DropColumn("DMS.DocumentEventAccessGroups", "AccessGroupTypeId");
            DropColumn("DMS.DocumentSendListAccessGroups", "AccessGroupTypeId");
        }
    }
}
