namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PersonsPosotion : DbMigration
    {
        public override void Up()
        {
            AddColumn("DMS.DictionaryAgentPersons", "Position", c => c.String(maxLength: 2000));
        }
        
        public override void Down()
        {
            DropColumn("DMS.DictionaryAgentPersons", "Position");
        }
    }
}
