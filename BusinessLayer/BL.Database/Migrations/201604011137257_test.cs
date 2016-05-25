namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class test : DbMigration
    {
        public override void Up()
        {
            AlterColumn("DMS.Documents", "RegistrationNumberSuffix", c => c.String(maxLength: 100));
            AlterColumn("DMS.Documents", "RegistrationNumberPrefix", c => c.String(maxLength: 100));
        }
        
        public override void Down()
        {
            AlterColumn("DMS.Documents", "RegistrationNumberPrefix", c => c.String(maxLength: 2000));
            AlterColumn("DMS.Documents", "RegistrationNumberSuffix", c => c.String(maxLength: 2000));
        }
    }
}
