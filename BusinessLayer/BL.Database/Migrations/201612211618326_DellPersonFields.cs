namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DellPersonFields : DbMigration
    {
        public override void Up()
        {
            AddForeignKey("DMS.DictionaryAgentPersons", "Id", "DMS.DictionaryAgentPeoples", "Id");
            DropColumn("DMS.DictionaryAgents", "Description");
            DropColumn("DMS.DictionaryAgents", "IsActive");
            DropColumn("DMS.DictionaryAgentPersons", "FullName");
            DropColumn("DMS.DictionaryAgentPersons", "LastName");
            DropColumn("DMS.DictionaryAgentPersons", "FirstName");
            DropColumn("DMS.DictionaryAgentPersons", "MiddleName");
            DropColumn("DMS.DictionaryAgentPersons", "TaxCode");
            DropColumn("DMS.DictionaryAgentPersons", "IsMale");
            DropColumn("DMS.DictionaryAgentPersons", "PassportSerial");
            DropColumn("DMS.DictionaryAgentPersons", "PassportNumber");
            DropColumn("DMS.DictionaryAgentPersons", "PassportText");
            DropColumn("DMS.DictionaryAgentPersons", "PassportDate");
            DropColumn("DMS.DictionaryAgentPersons", "BirthDate");
        }
        
        public override void Down()
        {
            AddColumn("DMS.DictionaryAgentPersons", "BirthDate", c => c.DateTime());
            AddColumn("DMS.DictionaryAgentPersons", "PassportDate", c => c.DateTime());
            AddColumn("DMS.DictionaryAgentPersons", "PassportText", c => c.String(maxLength: 2000));
            AddColumn("DMS.DictionaryAgentPersons", "PassportNumber", c => c.Int());
            AddColumn("DMS.DictionaryAgentPersons", "PassportSerial", c => c.String(maxLength: 2000));
            AddColumn("DMS.DictionaryAgentPersons", "IsMale", c => c.Boolean(nullable: false));
            AddColumn("DMS.DictionaryAgentPersons", "TaxCode", c => c.String(maxLength: 2000));
            AddColumn("DMS.DictionaryAgentPersons", "MiddleName", c => c.String(maxLength: 2000));
            AddColumn("DMS.DictionaryAgentPersons", "FirstName", c => c.String(maxLength: 2000));
            AddColumn("DMS.DictionaryAgentPersons", "LastName", c => c.String(maxLength: 2000));
            AddColumn("DMS.DictionaryAgentPersons", "FullName", c => c.String(maxLength: 400));
            AddColumn("DMS.DictionaryAgents", "IsActive", c => c.Boolean(nullable: false));
            AddColumn("DMS.DictionaryAgents", "Description", c => c.String(maxLength: 2000));
            DropForeignKey("DMS.DictionaryAgentPersons", "Id", "DMS.DictionaryAgentPeoples");
        }
    }
}
