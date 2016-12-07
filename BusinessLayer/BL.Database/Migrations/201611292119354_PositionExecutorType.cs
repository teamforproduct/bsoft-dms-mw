namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PositionExecutorType : DbMigration
    {
        public override void Up()
        {
            AddColumn("DMS.DictionaryPositions", "PositionExecutorTypeId", c => c.Int());
            AddColumn("DMS.DocumentEvents", "SourcePositionExecutorTypeId", c => c.Int());
            AddColumn("DMS.DocumentEvents", "TargetPositionExecutorTypeId", c => c.Int());
            AddColumn("DMS.DocumentSendLists", "SourcePositionExecutorTypeId", c => c.Int());
            AddColumn("DMS.DocumentSendLists", "TargetPositionExecutorTypeId", c => c.Int());
            CreateIndex("DMS.DictionaryPositions", "PositionExecutorTypeId");
            CreateIndex("DMS.DocumentEvents", "SourcePositionExecutorTypeId");
            CreateIndex("DMS.DocumentEvents", "TargetPositionExecutorTypeId");
            CreateIndex("DMS.DocumentSendLists", "SourcePositionExecutorTypeId");
            CreateIndex("DMS.DocumentSendLists", "TargetPositionExecutorTypeId");
            AddForeignKey("DMS.DictionaryPositions", "PositionExecutorTypeId", "DMS.DicPositionExecutorTypes", "Id");
            AddForeignKey("DMS.DocumentSendLists", "SourcePositionExecutorTypeId", "DMS.DicPositionExecutorTypes", "Id");
            AddForeignKey("DMS.DocumentSendLists", "TargetPositionExecutorTypeId", "DMS.DicPositionExecutorTypes", "Id");
            AddForeignKey("DMS.DocumentEvents", "SourcePositionExecutorTypeId", "DMS.DicPositionExecutorTypes", "Id");
            AddForeignKey("DMS.DocumentEvents", "TargetPositionExecutorTypeId", "DMS.DicPositionExecutorTypes", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("DMS.DocumentEvents", "TargetPositionExecutorTypeId", "DMS.DicPositionExecutorTypes");
            DropForeignKey("DMS.DocumentEvents", "SourcePositionExecutorTypeId", "DMS.DicPositionExecutorTypes");
            DropForeignKey("DMS.DocumentSendLists", "TargetPositionExecutorTypeId", "DMS.DicPositionExecutorTypes");
            DropForeignKey("DMS.DocumentSendLists", "SourcePositionExecutorTypeId", "DMS.DicPositionExecutorTypes");
            DropForeignKey("DMS.DictionaryPositions", "PositionExecutorTypeId", "DMS.DicPositionExecutorTypes");
            DropIndex("DMS.DocumentSendLists", new[] { "TargetPositionExecutorTypeId" });
            DropIndex("DMS.DocumentSendLists", new[] { "SourcePositionExecutorTypeId" });
            DropIndex("DMS.DocumentEvents", new[] { "TargetPositionExecutorTypeId" });
            DropIndex("DMS.DocumentEvents", new[] { "SourcePositionExecutorTypeId" });
            DropIndex("DMS.DictionaryPositions", new[] { "PositionExecutorTypeId" });
            DropColumn("DMS.DocumentSendLists", "TargetPositionExecutorTypeId");
            DropColumn("DMS.DocumentSendLists", "SourcePositionExecutorTypeId");
            DropColumn("DMS.DocumentEvents", "TargetPositionExecutorTypeId");
            DropColumn("DMS.DocumentEvents", "SourcePositionExecutorTypeId");
            DropColumn("DMS.DictionaryPositions", "PositionExecutorTypeId");
        }
    }
}
