namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IsRegistered : DbMigration
    {
        public override void Up()
        {
            AlterColumn("DMS.Documents", "IsRegistered", c => c.Boolean());
        }
        
        public override void Down()
        {
            AlterColumn("DMS.Documents", "IsRegistered", c => c.Boolean(nullable: false));
        }
    }
}
