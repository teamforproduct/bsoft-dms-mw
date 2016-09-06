namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class indexEvent1 : DbMigration
    {
        public override void Up()
        {
            CreateIndex("DMS.DocumentEvents", "SourcePositionId");
            CreateIndex("DMS.DocumentEvents", "TargetPositionId");
        }
        
        public override void Down()
        {
            DropIndex("DMS.DocumentEvents", new[] { "TargetPositionId" });
            DropIndex("DMS.DocumentEvents", new[] { "SourcePositionId" });
        }
    }
}
