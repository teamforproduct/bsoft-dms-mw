namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SystemIndex : DbMigration
    {
        public override void Up()
        {
            DropIndex("DMS.SystemActions", new[] { "ObjectId" });
            DropIndex("DMS.SystemFields", new[] { "ObjectId" });
            DropIndex("DMS.PropertyLinks", new[] { "PropertyId" });
            DropIndex("DMS.PropertyLinks", new[] { "ObjectId" });
            DropIndex("DMS.PropertyValues", new[] { "PropertyLinkId" });
            DropIndex("DMS.SystemSettings", new[] { "ExecutorAgentId" });
            CreateIndex("DMS.SystemActions", new[] { "ObjectId", "Code" }, unique: true, name: "IX_ObjectCode");
            CreateIndex("DMS.SystemObjects", "Code", unique: true);
            CreateIndex("DMS.SystemFields", new[] { "ObjectId", "Code" }, unique: true, name: "IX_ObjectCode");
            CreateIndex("DMS.SystemValueTypes", "Code", unique: true);
            CreateIndex("DMS.SystemLogs", "LogDate");
            CreateIndex("DMS.Properties", "Code", unique: true);
            CreateIndex("DMS.PropertyLinks", new[] { "ObjectId", "PropertyId", "Filers" }, unique: true, name: "IX_ObjectPropertyFilers");
            CreateIndex("DMS.PropertyLinks", "PropertyId");
            CreateIndex("DMS.PropertyValues", new[] { "PropertyLinkId", "RecordId" }, unique: true, name: "IX_PropertyLinkRecord");
            CreateIndex("DMS.SystemSettings", new[] { "Key", "ExecutorAgentId" }, unique: true, name: "IX_KeyExecutorAgent");
        }
        
        public override void Down()
        {
            DropIndex("DMS.SystemSettings", "IX_KeyExecutorAgent");
            DropIndex("DMS.PropertyValues", "IX_PropertyLinkRecord");
            DropIndex("DMS.PropertyLinks", new[] { "PropertyId" });
            DropIndex("DMS.PropertyLinks", "IX_ObjectPropertyFilers");
            DropIndex("DMS.Properties", new[] { "Code" });
            DropIndex("DMS.SystemLogs", new[] { "LogDate" });
            DropIndex("DMS.SystemValueTypes", new[] { "Code" });
            DropIndex("DMS.SystemFields", "IX_ObjectCode");
            DropIndex("DMS.SystemObjects", new[] { "Code" });
            DropIndex("DMS.SystemActions", "IX_ObjectCode");
            CreateIndex("DMS.SystemSettings", "ExecutorAgentId");
            CreateIndex("DMS.PropertyValues", "PropertyLinkId");
            CreateIndex("DMS.PropertyLinks", "ObjectId");
            CreateIndex("DMS.PropertyLinks", "PropertyId");
            CreateIndex("DMS.SystemFields", "ObjectId");
            CreateIndex("DMS.SystemActions", "ObjectId");
        }
    }
}
