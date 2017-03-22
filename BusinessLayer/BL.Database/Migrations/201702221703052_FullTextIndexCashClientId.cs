namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FullTextIndexCashClientId : DbMigration
    {
        public override void Up()
        {
            AddColumn("DMS.FullTextIndexCashes", "ClientId", c => c.Int(nullable: false));
            CreateIndex("DMS.FullTextIndexCashes", "ClientId");
        }
        
        public override void Down()
        {
            DropIndex("DMS.FullTextIndexCashes", new[] { "ClientId" });
            DropColumn("DMS.FullTextIndexCashes", "ClientId");
        }
    }
}
