namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixPositionExecutorType : DbMigration
    {
        public override void Up()
        {
            CreateIndex("DMS.DocumentEventAccesses", "PositionExecutorTypeId");
            AddForeignKey("DMS.DocumentEventAccesses", "PositionExecutorTypeId", "DMS.DicPositionExecutorTypes", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("DMS.DocumentEventAccesses", "PositionExecutorTypeId", "DMS.DicPositionExecutorTypes");
            DropIndex("DMS.DocumentEventAccesses", new[] { "PositionExecutorTypeId" });
        }
    }
}
