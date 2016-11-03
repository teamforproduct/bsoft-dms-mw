namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddImageToAgents : DbMigration
    {
        public override void Up()
        {
            AddColumn("DMS.DictionaryAgents", "Image", c => c.Binary());
            DropColumn("DMS.DictionaryAgentUsers", "Picture");
        }
        
        public override void Down()
        {
            AddColumn("DMS.DictionaryAgentUsers", "Picture", c => c.Binary());
            DropColumn("DMS.DictionaryAgents", "Image");
        }
    }
}
