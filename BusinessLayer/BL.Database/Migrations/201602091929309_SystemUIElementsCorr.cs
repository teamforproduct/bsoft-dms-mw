namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SystemUIElementsCorr : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SystemUIElements", "Label", c => c.String());
            AddColumn("dbo.SystemUIElements", "SelectFilter", c => c.String());
            DropColumn("dbo.SystemUIElements", "Lable");
            DropColumn("dbo.SystemUIElements", "SelectFiler");
        }
        
        public override void Down()
        {
            AddColumn("dbo.SystemUIElements", "SelectFiler", c => c.String());
            AddColumn("dbo.SystemUIElements", "Lable", c => c.String());
            DropColumn("dbo.SystemUIElements", "SelectFilter");
            DropColumn("dbo.SystemUIElements", "Label");
        }
    }
}
