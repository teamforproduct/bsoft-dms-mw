namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FullTextIndexCashSet : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "DMS.FullTextIndexCashes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ObjectId = c.Int(nullable: false),
                        ObjectType = c.Int(nullable: false),
                        OperationType = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("DMS.FullTextIndexCashes");
        }
    }
}
