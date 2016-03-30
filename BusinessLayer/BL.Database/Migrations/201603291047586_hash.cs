namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hash : DbMigration
    {
        public override void Up()
        {
            AddColumn("DMS.DocumentPapers", "IsInWork", c => c.Boolean(nullable: false));
            AddColumn("DMS.DocumentSubscriptions", "FullHash", c => c.String(maxLength: 2000));
        }
        
        public override void Down()
        {
            DropColumn("DMS.DocumentSubscriptions", "FullHash");
            DropColumn("DMS.DocumentPapers", "IsInWork");
        }
    }
}
