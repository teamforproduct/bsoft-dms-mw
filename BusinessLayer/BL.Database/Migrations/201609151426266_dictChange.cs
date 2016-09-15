namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dictChange : DbMigration
    {
        public override void Up()
        {
            DropIndex("DMS.DictionaryAgentContacts", "IX_AgentContactTypeContact");
            DropIndex("DMS.DictionaryAgentContacts", new[] { "ClientId" });
            AddColumn("DMS.DictionaryDepartments", "FullPath", c => c.String(maxLength: 2000));
            AddColumn("DMS.DictionaryAddressTypes", "SpecCode", c => c.String(maxLength: 2000));
            AddColumn("DMS.DictionaryContactTypes", "SpecCode", c => c.String(maxLength: 2000));
            AddColumn("DMS.DictionaryAgentUsers", "Login", c => c.String(maxLength: 256));
            AddColumn("DMS.DictionaryAgentUsers", "PasswordHash", c => c.String(maxLength: 2000));
            AddColumn("DMS.DictionaryAgentUsers", "Picture", c => c.Binary());
            CreateIndex("DMS.DictionaryAgentContacts", new[] { "AgentId", "ContactTypeId", "Contact" }, unique: true, name: "IX_AgentContactTypeContact");
            DropColumn("DMS.DictionaryAgents", "IsCompany");
            DropColumn("DMS.DictionaryAgents", "IsIndividual");
            DropColumn("DMS.DictionaryAgents", "IsEmployee");
            DropColumn("DMS.DictionaryAgents", "IsBank");
            DropColumn("DMS.DictionaryAgents", "IsUser");
            DropColumn("DMS.DictionaryAgentContacts", "ClientId");
        }
        
        public override void Down()
        {
            AddColumn("DMS.DictionaryAgentContacts", "ClientId", c => c.Int(nullable: false));
            AddColumn("DMS.DictionaryAgents", "IsUser", c => c.Boolean(nullable: false));
            AddColumn("DMS.DictionaryAgents", "IsBank", c => c.Boolean(nullable: false));
            AddColumn("DMS.DictionaryAgents", "IsEmployee", c => c.Boolean(nullable: false));
            AddColumn("DMS.DictionaryAgents", "IsIndividual", c => c.Boolean(nullable: false));
            AddColumn("DMS.DictionaryAgents", "IsCompany", c => c.Boolean(nullable: false));
            DropIndex("DMS.DictionaryAgentContacts", "IX_AgentContactTypeContact");
            DropColumn("DMS.DictionaryAgentUsers", "Picture");
            DropColumn("DMS.DictionaryAgentUsers", "PasswordHash");
            DropColumn("DMS.DictionaryAgentUsers", "Login");
            DropColumn("DMS.DictionaryContactTypes", "SpecCode");
            DropColumn("DMS.DictionaryAddressTypes", "SpecCode");
            DropColumn("DMS.DictionaryDepartments", "FullPath");
            CreateIndex("DMS.DictionaryAgentContacts", "ClientId");
            CreateIndex("DMS.DictionaryAgentContacts", new[] { "AgentId", "ContactTypeId", "Contact", "ClientId" }, unique: true, name: "IX_AgentContactTypeContact");
        }
    }
}
