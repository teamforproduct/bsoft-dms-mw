namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PositionExecutorTypeId : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "DMS.Documents", name: "PositionExecutorTypeId", newName: "ExecutorPositionExeTypeId");
            RenameIndex(table: "DMS.Documents", name: "IX_PositionExecutorTypeId", newName: "IX_ExecutorPositionExeTypeId");
            AddColumn("DMS.DocumentTasks", "PositionExecutorTypeId", c => c.Int());
            AddColumn("DMS.DocumentSubscriptions", "CertificatePositionExecutorTypeId", c => c.Int());
            AddColumn("DMS.DocumentFiles", "ExecutorPositionExeTypeId", c => c.Int());
            CreateIndex("DMS.DocumentTasks", "PositionExecutorTypeId");
            CreateIndex("DMS.DocumentSubscriptions", "CertificatePositionExecutorTypeId");
            CreateIndex("DMS.DocumentFiles", "ExecutorPositionExeTypeId");
            AddForeignKey("DMS.DocumentTasks", "PositionExecutorTypeId", "DMS.DicPositionExecutorTypes", "Id");
            AddForeignKey("DMS.DocumentSubscriptions", "CertificatePositionExecutorTypeId", "DMS.DicPositionExecutorTypes", "Id");
            AddForeignKey("DMS.DocumentFiles", "ExecutorPositionExeTypeId", "DMS.DicPositionExecutorTypes", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("DMS.DocumentFiles", "ExecutorPositionExeTypeId", "DMS.DicPositionExecutorTypes");
            DropForeignKey("DMS.DocumentSubscriptions", "CertificatePositionExecutorTypeId", "DMS.DicPositionExecutorTypes");
            DropForeignKey("DMS.DocumentTasks", "PositionExecutorTypeId", "DMS.DicPositionExecutorTypes");
            DropIndex("DMS.DocumentFiles", new[] { "ExecutorPositionExeTypeId" });
            DropIndex("DMS.DocumentSubscriptions", new[] { "CertificatePositionExecutorTypeId" });
            DropIndex("DMS.DocumentTasks", new[] { "PositionExecutorTypeId" });
            DropColumn("DMS.DocumentFiles", "ExecutorPositionExeTypeId");
            DropColumn("DMS.DocumentSubscriptions", "CertificatePositionExecutorTypeId");
            DropColumn("DMS.DocumentTasks", "PositionExecutorTypeId");
            RenameIndex(table: "DMS.Documents", name: "IX_ExecutorPositionExeTypeId", newName: "IX_PositionExecutorTypeId");
            RenameColumn(table: "DMS.Documents", name: "ExecutorPositionExeTypeId", newName: "PositionExecutorTypeId");
        }
    }
}
