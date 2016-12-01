namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DocumentsPositionExecutorType : DbMigration
    {
        public override void Up()
        {
            AddColumn("DMS.Documents", "PositionExecutorTypeId", c => c.Int());
            CreateIndex("DMS.Documents", "PositionExecutorTypeId");
            AddForeignKey("DMS.Documents", "PositionExecutorTypeId", "DMS.DicPositionExecutorTypes", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("DMS.Documents", "PositionExecutorTypeId", "DMS.DicPositionExecutorTypes");
            DropIndex("DMS.Documents", new[] { "PositionExecutorTypeId" });
            DropColumn("DMS.Documents", "PositionExecutorTypeId");
        }
    }
}
