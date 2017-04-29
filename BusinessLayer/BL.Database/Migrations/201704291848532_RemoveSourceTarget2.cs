namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveSourceTarget2 : DbMigration
    {
        public override void Up()
        {
            DropColumn("DMS.DocumentSendLists", "SourcePositionId");
            DropColumn("DMS.DocumentSendLists", "SourcePositionExecutorAgentId");
            DropColumn("DMS.DocumentSendLists", "SourcePositionExecutorTypeId");
            DropColumn("DMS.DocumentSendLists", "SourceAgentId");
            DropColumn("DMS.DocumentSendLists", "TargetPositionId");
            DropColumn("DMS.DocumentSendLists", "TargetPositionExecutorAgentId");
            DropColumn("DMS.DocumentSendLists", "TargetPositionExecutorTypeId");
            DropColumn("DMS.DocumentSendLists", "TargetAgentId");
        }
        
        public override void Down()
        {
            AddColumn("DMS.DocumentSendLists", "TargetAgentId", c => c.Int());
            AddColumn("DMS.DocumentSendLists", "TargetPositionExecutorTypeId", c => c.Int());
            AddColumn("DMS.DocumentSendLists", "TargetPositionExecutorAgentId", c => c.Int());
            AddColumn("DMS.DocumentSendLists", "TargetPositionId", c => c.Int());
            AddColumn("DMS.DocumentSendLists", "SourceAgentId", c => c.Int(nullable: false));
            AddColumn("DMS.DocumentSendLists", "SourcePositionExecutorTypeId", c => c.Int());
            AddColumn("DMS.DocumentSendLists", "SourcePositionExecutorAgentId", c => c.Int());
            AddColumn("DMS.DocumentSendLists", "SourcePositionId", c => c.Int(nullable: false));
        }
    }
}
