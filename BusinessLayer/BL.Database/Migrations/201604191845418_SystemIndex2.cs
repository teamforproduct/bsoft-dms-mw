namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SystemIndex2 : DbMigration
    {
        public override void Up()
        {
            DropIndex("DMS.SystemUIElements", new[] { "ActionId" });
            CreateIndex("DMS.SystemUIElements", new[] { "ActionId", "Code" }, unique: true, name: "IX_ActionCode");
        }
        
        public override void Down()
        {
            DropIndex("DMS.SystemUIElements", "IX_ActionCode");
            CreateIndex("DMS.SystemUIElements", "ActionId");
        }
    }
}
