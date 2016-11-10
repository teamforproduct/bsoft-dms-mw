namespace BL.Database.Migrations
{
    using DatabaseContext;
    using System;
    using System.Data.Entity.Migrations;

    public partial class RegJournalPositions : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "DMS.AdminRegistrationJournalPositions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PositionId = c.Int(nullable: false),
                        RegJournalId = c.Int(nullable: false),
                        RegJournalAccessTypeId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DMS.DictionaryPositions", t => t.PositionId)
                .ForeignKey("DMS.DictionaryRegistrationJournals", t => t.RegJournalId)
                .ForeignKey("DMS.DicRegJournalAccessTypes", t => t.RegJournalAccessTypeId)
                .Index(t => new { t.PositionId, t.RegJournalId, t.RegJournalAccessTypeId }, unique: true, name: "IX_JournalPositionType")
                .Index(t => t.RegJournalId)
                .Index(t => t.RegJournalAccessTypeId);
            
           CreateTable(
                "DMS.DicRegJournalAccessTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(maxLength: 400),
                        Name = c.String(maxLength: 400),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Code, unique: true)
                .Index(t => t.Name, unique: true);

            Sql("SET IDENTITY_INSERT [DMS].[DicRegJournalAccessTypes] ON");
            Sql("insert [DMS].[DicRegJournalAccessTypes] (Id, Code, [Name], LastChangeUserId, LastChangeDate) select 1, 'View', '##l@DictionaryRegistrationJournalAccessTypes:View@l##',-1,getdate()");
            Sql("insert [DMS].[DicRegJournalAccessTypes] (Id, Code, [Name], LastChangeUserId, LastChangeDate) select 2, 'Registration', '##l@DictionaryRegistrationJournalAccessTypes:Registration@l##',-1,getdate()");
            Sql("SET IDENTITY_INSERT [DMS].[DicRegJournalAccessTypes] OFF");
            //context.DictionaryRegistrationJournalAccessTypesSet.AddRange(DmsDbImportData.GetDictionaryRegistrationJournalAccessTypes());

        }
        
        public override void Down()
        {
            DropForeignKey("DMS.AdminRegistrationJournalPositions", "RegJournalAccessTypeId", "DMS.DicRegJournalAccessTypes");
            DropForeignKey("DMS.AdminRegistrationJournalPositions", "RegJournalId", "DMS.DictionaryRegistrationJournals");
            DropForeignKey("DMS.AdminRegistrationJournalPositions", "PositionId", "DMS.DictionaryPositions");
            DropIndex("DMS.DicRegJournalAccessTypes", new[] { "Name" });
            DropIndex("DMS.DicRegJournalAccessTypes", new[] { "Code" });
            DropIndex("DMS.AdminRegistrationJournalPositions", new[] { "RegJournalAccessTypeId" });
            DropIndex("DMS.AdminRegistrationJournalPositions", new[] { "RegJournalId" });
            DropIndex("DMS.AdminRegistrationJournalPositions", "IX_JournalPositionType");
            DropTable("DMS.DicRegJournalAccessTypes");
            DropTable("DMS.AdminRegistrationJournalPositions");
        }
    }
}
