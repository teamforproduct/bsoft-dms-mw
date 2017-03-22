namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AccessTypeId : DbMigration
    {
        public override void Up()
        {
            AddColumn("DMS.DocumentEventAccesses", "AccessTypeId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("DMS.DocumentEventAccesses", "AccessTypeId");
        }
    }
}
