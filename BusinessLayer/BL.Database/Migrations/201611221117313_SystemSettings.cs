namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SystemSettings : DbMigration
    {
        public override void Up()
        {
            DropIndex("DMS.SystemSettings", new[] { "ValueType" });
            RenameColumn(table: "DMS.SystemSettings", name: "ValueType", newName: "ValueTypeId");
            CreateTable(
                "DMS.DictionarySettingTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(maxLength: 400),
                        Name = c.String(maxLength: 400),
                        Order = c.Int(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Code, unique: true)
                .Index(t => t.Name, unique: true);
            
            AddColumn("DMS.SystemSettings", "Name", c => c.String(maxLength: 400));
            AddColumn("DMS.SystemSettings", "Description", c => c.String(maxLength: 2000));
            AddColumn("DMS.SystemSettings", "SettingTypeId", c => c.Int());
            AddColumn("DMS.SystemSettings", "Order", c => c.Int(nullable: false));
            AddColumn("DMS.SystemSettings", "AccessType", c => c.Int(nullable: false));
            AlterColumn("DMS.SystemSettings", "ValueTypeId", c => c.Int());
            CreateIndex("DMS.SystemSettings", "ValueTypeId");
            CreateIndex("DMS.SystemSettings", "SettingTypeId");
            AddForeignKey("DMS.SystemSettings", "SettingTypeId", "DMS.SystemValueTypes", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("DMS.SystemSettings", "SettingTypeId", "DMS.SystemValueTypes");
            DropIndex("DMS.SystemSettings", new[] { "SettingTypeId" });
            DropIndex("DMS.SystemSettings", new[] { "ValueTypeId" });
            DropIndex("DMS.DictionarySettingTypes", new[] { "Name" });
            DropIndex("DMS.DictionarySettingTypes", new[] { "Code" });
            AlterColumn("DMS.SystemSettings", "ValueTypeId", c => c.Int(nullable: false));
            DropColumn("DMS.SystemSettings", "AccessType");
            DropColumn("DMS.SystemSettings", "Order");
            DropColumn("DMS.SystemSettings", "SettingTypeId");
            DropColumn("DMS.SystemSettings", "Description");
            DropColumn("DMS.SystemSettings", "Name");
            DropTable("DMS.DictionarySettingTypes");
            RenameColumn(table: "DMS.SystemSettings", name: "ValueTypeId", newName: "ValueType");
            CreateIndex("DMS.SystemSettings", "ValueType");
        }
    }
}
