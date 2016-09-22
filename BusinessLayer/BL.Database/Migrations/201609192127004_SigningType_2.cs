namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SigningType_2 : DbMigration
    {
        public override void Up()
        {
            DropIndex("DMS.DocumentSubscriptions", new[] { "SigningTypeId" });
            AlterColumn("DMS.DocumentSubscriptions", "SigningTypeId", c => c.Int(nullable: false));
            CreateIndex("DMS.DocumentSubscriptions", "SigningTypeId");
        }
        
        public override void Down()
        {
            DropIndex("DMS.DocumentSubscriptions", new[] { "SigningTypeId" });
            AlterColumn("DMS.DocumentSubscriptions", "SigningTypeId", c => c.Int());
            CreateIndex("DMS.DocumentSubscriptions", "SigningTypeId");
        }
    }
}
