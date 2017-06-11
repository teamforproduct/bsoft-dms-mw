namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveLangLabels : DbMigration
    {
        public override void Up()
        {
            DropIndex("DMS.AdminAccessLevels", new[] { "Name" });
            DropIndex("DMS.DicPositionExecutorTypes", new[] { "Code" });
            DropIndex("DMS.DicPositionExecutorTypes", new[] { "Name" });
            DropIndex("DMS.SystemAccessTypes", new[] { "Code" });
            DropIndex("DMS.SystemActions", "IX_ObjectCode");
            DropIndex("DMS.SystemObjects", new[] { "Code" });
            DropIndex("DMS.SystemFields", "IX_ObjectCode");
            DropIndex("DMS.SystemValueTypes", new[] { "Code" });
            DropIndex("DMS.AdminRoleTypes", new[] { "Code" });
            DropIndex("DMS.AdminRoleTypes", new[] { "Name" });
            DropIndex("DMS.DictionaryDocumentDirections", new[] { "Code" });
            DropIndex("DMS.DictionaryDocumentDirections", new[] { "Name" });
            DropIndex("DMS.DictionarySendTypes", new[] { "Name" });
            DropIndex("DMS.DictionarySubordinationTypes", new[] { "Name" });
            DropIndex("DMS.DictionaryStageTypes", new[] { "Name" });
            DropIndex("DMS.DictionaryEventTypes", new[] { "Name" });
            DropIndex("DMS.DictionaryImportanceEventTypes", new[] { "Name" });
            DropIndex("DMS.DictionaryFileTypes", new[] { "Name" });
            DropIndex("DMS.DictionaryResultTypes", new[] { "Name" });
            DropIndex("DMS.DictionaryLinkTypes", new[] { "Name" });
            DropIndex("DMS.DicRegJournalAccessTypes", new[] { "Code" });
            DropIndex("DMS.DicRegJournalAccessTypes", new[] { "Name" });
            DropIndex("DMS.DictionarySigningTypes", new[] { "Name" });
            DropIndex("DMS.DictionarySubscriptionStates", new[] { "Code" });
            DropIndex("DMS.DictionarySubscriptionStates", new[] { "Name" });
            DropIndex("DMS.DictionaryResidentTypes", "IX_Name");
            DropIndex("DMS.DictionarySettingTypes", new[] { "Code" });
            DropIndex("DMS.DictionarySettingTypes", new[] { "Name" });
            DropIndex("DMS.SystemFormats", new[] { "Code" });
            DropIndex("DMS.SystemFormulas", new[] { "Code" });
            DropIndex("DMS.SystemPatterns", new[] { "Code" });
            DropIndex("DMS.SystemUIElements", "IX_ActionCode");
            CreateTable(
                "DMS.SystemMenus",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MenuTypeId = c.Int(nullable: false),
                        ActionId = c.Int(nullable: false),
                        Order = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.SystemActions", t => t.ActionId)
                .Index(t => t.ActionId);
            
            AddColumn("DMS.DictionaryResultTypes", "Code", c => c.String(maxLength: 400));
            AddColumn("DMS.DictionaryLinkTypes", "Code", c => c.String(maxLength: 400));
            AddColumn("DMS.DictionaryResidentTypes", "Code", c => c.String(maxLength: 400));
            AlterColumn("DMS.AdminAccessLevels", "Code", c => c.String(maxLength: 400));
            AlterColumn("DMS.DictionarySendTypes", "Code", c => c.String(maxLength: 400));
            AlterColumn("DMS.DictionaryStageTypes", "Code", c => c.String(maxLength: 400));
            AlterColumn("DMS.DictionaryEventTypes", "Code", c => c.String(maxLength: 400));
            AlterColumn("DMS.DictionaryImportanceEventTypes", "Code", c => c.String(maxLength: 400));
            AlterColumn("DMS.DictionaryResidentTypes", "Name", c => c.String(maxLength: 2000));
            CreateIndex("DMS.SystemActions", "ObjectId");
            CreateIndex("DMS.SystemFields", "ObjectId");
            CreateIndex("DMS.DictionaryResidentTypes", new[] { "Code", "ClientId" }, unique: true, name: "IX_Code");
            CreateIndex("DMS.DictionaryResidentTypes", new[] { "Name", "ClientId" }, unique: true, name: "IX_Name");
            CreateIndex("DMS.SystemUIElements", "ActionId");
            DropColumn("DMS.AdminAccessLevels", "Name");
            DropColumn("DMS.DicPositionExecutorTypes", "Name");
            DropColumn("DMS.DicPositionExecutorTypes", "Description");
            DropColumn("DMS.SystemAccessTypes", "Name");
            DropColumn("DMS.SystemActions", "Description");
            DropColumn("DMS.SystemActions", "Category");
            DropColumn("DMS.SystemObjects", "Description");
            DropColumn("DMS.SystemFields", "Description");
            DropColumn("DMS.SystemValueTypes", "Description");
            DropColumn("DMS.AdminRoleTypes", "Name");
            DropColumn("DMS.DictionaryDocumentDirections", "Name");
            DropColumn("DMS.DictionarySendTypes", "Name");
            DropColumn("DMS.DictionarySubordinationTypes", "Name");
            DropColumn("DMS.DictionaryStageTypes", "Name");
            DropColumn("DMS.DictionaryEventTypes", "Name");
            DropColumn("DMS.DictionaryImportanceEventTypes", "Name");
            DropColumn("DMS.DictionaryFileTypes", "Name");
            DropColumn("DMS.DictionaryResultTypes", "Name");
            DropColumn("DMS.DictionaryLinkTypes", "Name");
            DropColumn("DMS.DicRegJournalAccessTypes", "Name");
            DropColumn("DMS.DictionarySigningTypes", "Name");
            DropColumn("DMS.DictionarySubscriptionStates", "Name");
            DropColumn("DMS.DictionarySettingTypes", "Name");
            DropColumn("DMS.SystemSettings", "Name");
            DropColumn("DMS.SystemSettings", "Description");
            DropColumn("DMS.SystemFormats", "Name");
            DropColumn("DMS.SystemFormats", "Description");
            DropColumn("DMS.SystemFormulas", "Name");
            DropColumn("DMS.SystemFormulas", "Description");
            DropColumn("DMS.SystemPatterns", "Name");
            DropColumn("DMS.SystemPatterns", "Description");
            DropColumn("DMS.SystemUIElements", "Description");
            DropColumn("DMS.SystemUIElements", "Label");
            DropColumn("DMS.SystemUIElements", "Hint");
        }
        
        public override void Down()
        {
            AddColumn("DMS.SystemUIElements", "Hint", c => c.String(maxLength: 2000));
            AddColumn("DMS.SystemUIElements", "Label", c => c.String(maxLength: 2000));
            AddColumn("DMS.SystemUIElements", "Description", c => c.String(maxLength: 2000));
            AddColumn("DMS.SystemPatterns", "Description", c => c.String(maxLength: 2000));
            AddColumn("DMS.SystemPatterns", "Name", c => c.String(maxLength: 2000));
            AddColumn("DMS.SystemFormulas", "Description", c => c.String(maxLength: 2000));
            AddColumn("DMS.SystemFormulas", "Name", c => c.String(maxLength: 2000));
            AddColumn("DMS.SystemFormats", "Description", c => c.String(maxLength: 2000));
            AddColumn("DMS.SystemFormats", "Name", c => c.String(maxLength: 2000));
            AddColumn("DMS.SystemSettings", "Description", c => c.String(maxLength: 2000));
            AddColumn("DMS.SystemSettings", "Name", c => c.String(maxLength: 400));
            AddColumn("DMS.DictionarySettingTypes", "Name", c => c.String(maxLength: 400));
            AddColumn("DMS.DictionarySubscriptionStates", "Name", c => c.String(maxLength: 400));
            AddColumn("DMS.DictionarySigningTypes", "Name", c => c.String(maxLength: 400));
            AddColumn("DMS.DicRegJournalAccessTypes", "Name", c => c.String(maxLength: 400));
            AddColumn("DMS.DictionaryLinkTypes", "Name", c => c.String(maxLength: 400));
            AddColumn("DMS.DictionaryResultTypes", "Name", c => c.String(maxLength: 2000));
            AddColumn("DMS.DictionaryFileTypes", "Name", c => c.String(maxLength: 400));
            AddColumn("DMS.DictionaryImportanceEventTypes", "Name", c => c.String(maxLength: 2000));
            AddColumn("DMS.DictionaryEventTypes", "Name", c => c.String(maxLength: 2000));
            AddColumn("DMS.DictionaryStageTypes", "Name", c => c.String(maxLength: 2000));
            AddColumn("DMS.DictionarySubordinationTypes", "Name", c => c.String(maxLength: 400));
            AddColumn("DMS.DictionarySendTypes", "Name", c => c.String(maxLength: 2000));
            AddColumn("DMS.DictionaryDocumentDirections", "Name", c => c.String(maxLength: 400));
            AddColumn("DMS.AdminRoleTypes", "Name", c => c.String(maxLength: 400));
            AddColumn("DMS.SystemValueTypes", "Description", c => c.String(maxLength: 2000));
            AddColumn("DMS.SystemFields", "Description", c => c.String(maxLength: 2000));
            AddColumn("DMS.SystemObjects", "Description", c => c.String(maxLength: 2000));
            AddColumn("DMS.SystemActions", "Category", c => c.String(maxLength: 2000));
            AddColumn("DMS.SystemActions", "Description", c => c.String(maxLength: 2000));
            AddColumn("DMS.SystemAccessTypes", "Name", c => c.String(maxLength: 2000));
            AddColumn("DMS.DicPositionExecutorTypes", "Description", c => c.String(maxLength: 400));
            AddColumn("DMS.DicPositionExecutorTypes", "Name", c => c.String(maxLength: 400));
            AddColumn("DMS.AdminAccessLevels", "Name", c => c.String(maxLength: 400));
            DropForeignKey("DMS.SystemMenus", "ActionId", "DMS.SystemActions");
            DropIndex("DMS.SystemUIElements", new[] { "ActionId" });
            DropIndex("DMS.SystemMenus", new[] { "ActionId" });
            DropIndex("DMS.DictionaryResidentTypes", "IX_Name");
            DropIndex("DMS.DictionaryResidentTypes", "IX_Code");
            DropIndex("DMS.SystemFields", new[] { "ObjectId" });
            DropIndex("DMS.SystemActions", new[] { "ObjectId" });
            AlterColumn("DMS.DictionaryResidentTypes", "Name", c => c.String(maxLength: 400));
            AlterColumn("DMS.DictionaryImportanceEventTypes", "Code", c => c.String(maxLength: 2000));
            AlterColumn("DMS.DictionaryEventTypes", "Code", c => c.String(maxLength: 2000));
            AlterColumn("DMS.DictionaryStageTypes", "Code", c => c.String(maxLength: 2000));
            AlterColumn("DMS.DictionarySendTypes", "Code", c => c.String(maxLength: 2000));
            AlterColumn("DMS.AdminAccessLevels", "Code", c => c.String(maxLength: 2000));
            DropColumn("DMS.DictionaryResidentTypes", "Code");
            DropColumn("DMS.DictionaryLinkTypes", "Code");
            DropColumn("DMS.DictionaryResultTypes", "Code");
            DropTable("DMS.SystemMenus");
            CreateIndex("DMS.SystemUIElements", new[] { "ActionId", "Code" }, unique: true, name: "IX_ActionCode");
            CreateIndex("DMS.SystemPatterns", "Code", unique: true);
            CreateIndex("DMS.SystemFormulas", "Code", unique: true);
            CreateIndex("DMS.SystemFormats", "Code", unique: true);
            CreateIndex("DMS.DictionarySettingTypes", "Name", unique: true);
            CreateIndex("DMS.DictionarySettingTypes", "Code", unique: true);
            CreateIndex("DMS.DictionaryResidentTypes", new[] { "Name", "ClientId" }, unique: true, name: "IX_Name");
            CreateIndex("DMS.DictionarySubscriptionStates", "Name", unique: true);
            CreateIndex("DMS.DictionarySubscriptionStates", "Code", unique: true);
            CreateIndex("DMS.DictionarySigningTypes", "Name", unique: true);
            CreateIndex("DMS.DicRegJournalAccessTypes", "Name", unique: true);
            CreateIndex("DMS.DicRegJournalAccessTypes", "Code", unique: true);
            CreateIndex("DMS.DictionaryLinkTypes", "Name", unique: true);
            CreateIndex("DMS.DictionaryResultTypes", "Name", unique: true);
            CreateIndex("DMS.DictionaryFileTypes", "Name", unique: true);
            CreateIndex("DMS.DictionaryImportanceEventTypes", "Name", unique: true);
            CreateIndex("DMS.DictionaryEventTypes", "Name", unique: true);
            CreateIndex("DMS.DictionaryStageTypes", "Name", unique: true);
            CreateIndex("DMS.DictionarySubordinationTypes", "Name", unique: true);
            CreateIndex("DMS.DictionarySendTypes", "Name", unique: true);
            CreateIndex("DMS.DictionaryDocumentDirections", "Name", unique: true);
            CreateIndex("DMS.DictionaryDocumentDirections", "Code", unique: true);
            CreateIndex("DMS.AdminRoleTypes", "Name", unique: true);
            CreateIndex("DMS.AdminRoleTypes", "Code", unique: true);
            CreateIndex("DMS.SystemValueTypes", "Code", unique: true);
            CreateIndex("DMS.SystemFields", new[] { "ObjectId", "Code" }, unique: true, name: "IX_ObjectCode");
            CreateIndex("DMS.SystemObjects", "Code", unique: true);
            CreateIndex("DMS.SystemActions", new[] { "ObjectId", "Code" }, unique: true, name: "IX_ObjectCode");
            CreateIndex("DMS.SystemAccessTypes", "Code", unique: true);
            CreateIndex("DMS.DicPositionExecutorTypes", "Name", unique: true);
            CreateIndex("DMS.DicPositionExecutorTypes", "Code", unique: true);
            CreateIndex("DMS.AdminAccessLevels", "Name", unique: true);
        }
    }
}
