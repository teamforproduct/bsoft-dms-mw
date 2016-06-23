namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TagNameMaxLength : DbMigration
    {
        public override void Up()
        {
            AlterColumn("DMS.DictionaryTags", "Name", c => c.String(maxLength: 900));
        }
        
        public override void Down()
        {
            AlterColumn("DMS.DictionaryTags", "Name", c => c.String(maxLength: 2000));
        }
    }
}
