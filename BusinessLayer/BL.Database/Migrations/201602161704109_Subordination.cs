namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Subordination : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.AdminSubordinations", name: "AddresseePositionId", newName: "SourcePositionId");
            RenameColumn(table: "dbo.AdminSubordinations", name: "PositionId", newName: "TargetPositionId");
            RenameIndex(table: "dbo.AdminSubordinations", name: "IX_AddresseePositionId", newName: "IX_SourcePositionId");
            RenameIndex(table: "dbo.AdminSubordinations", name: "IX_PositionId", newName: "IX_TargetPositionId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.AdminSubordinations", name: "IX_TargetPositionId", newName: "IX_PositionId");
            RenameIndex(table: "dbo.AdminSubordinations", name: "IX_SourcePositionId", newName: "IX_AddresseePositionId");
            RenameColumn(table: "dbo.AdminSubordinations", name: "TargetPositionId", newName: "PositionId");
            RenameColumn(table: "dbo.AdminSubordinations", name: "SourcePositionId", newName: "AddresseePositionId");
        }
    }
}
