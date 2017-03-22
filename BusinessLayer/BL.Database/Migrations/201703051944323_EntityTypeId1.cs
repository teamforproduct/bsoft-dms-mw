namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EntityTypeId1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("DMS.DocumentSendLists", "EntityTypeId", c => c.Int(nullable: false));
            CreateIndex("DMS.DocumentSendLists", "EntityTypeId");
        }
        
        public override void Down()
        {
            DropIndex("DMS.DocumentSendLists", new[] { "EntityTypeId" });
            DropColumn("DMS.DocumentSendLists", "EntityTypeId");
        }
    }
}
