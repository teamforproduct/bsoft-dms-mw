namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DictionaryPositionExecutorTypesSuffix : DbMigration
    {
        public override void Up()
        {
            AddColumn("DMS.DicPositionExecutorTypes", "Suffix", c => c.String(maxLength: 400));
        }
        
        public override void Down()
        {
            DropColumn("DMS.DicPositionExecutorTypes", "Suffix");
        }
    }
}
