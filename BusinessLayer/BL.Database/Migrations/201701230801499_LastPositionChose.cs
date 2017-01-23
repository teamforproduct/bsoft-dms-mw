namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LastPositionChose : DbMigration
    {
        public override void Up()
        {
            AddColumn("DMS.DictionaryAgentUsers", "LastPositionChose", c => c.String(maxLength: 2000));
        }
        
        public override void Down()
        {
            DropColumn("DMS.DictionaryAgentUsers", "LastPositionChose");
        }
    }
}
