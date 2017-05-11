namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveSourceTarget3 : DbMigration
    {
        public override void Up()
        {
            DropColumn("DMS.DocumentEvents", "SourcePositionId");
            DropColumn("DMS.DocumentEvents", "SourcePositionExecutorAgentId");
            DropColumn("DMS.DocumentEvents", "SourcePositionExecutorTypeId");
            DropColumn("DMS.DocumentEvents", "SourceAgentId");
            DropColumn("DMS.DocumentEvents", "TargetPositionId");
            DropColumn("DMS.DocumentEvents", "TargetPositionExecutorAgentId");
            DropColumn("DMS.DocumentEvents", "TargetPositionExecutorTypeId");
            DropColumn("DMS.DocumentEvents", "TargetAgentId");
            DropColumn("DMS.DocumentEvents", "SendDate");
            DropColumn("DMS.DocumentEvents", "ReadDate");
            DropColumn("DMS.DocumentEvents", "ReadAgentId");
        }
        
        public override void Down()
        {
            AddColumn("DMS.DocumentEvents", "ReadAgentId", c => c.Int());
            AddColumn("DMS.DocumentEvents", "ReadDate", c => c.DateTime());
            AddColumn("DMS.DocumentEvents", "SendDate", c => c.DateTime());
            AddColumn("DMS.DocumentEvents", "TargetAgentId", c => c.Int());
            AddColumn("DMS.DocumentEvents", "TargetPositionExecutorTypeId", c => c.Int());
            AddColumn("DMS.DocumentEvents", "TargetPositionExecutorAgentId", c => c.Int());
            AddColumn("DMS.DocumentEvents", "TargetPositionId", c => c.Int());
            AddColumn("DMS.DocumentEvents", "SourceAgentId", c => c.Int());
            AddColumn("DMS.DocumentEvents", "SourcePositionExecutorTypeId", c => c.Int());
            AddColumn("DMS.DocumentEvents", "SourcePositionExecutorAgentId", c => c.Int());
            AddColumn("DMS.DocumentEvents", "SourcePositionId", c => c.Int());
        }
    }
}
