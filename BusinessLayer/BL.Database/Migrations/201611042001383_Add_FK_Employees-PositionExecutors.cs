namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_FK_EmployeesPositionExecutors : DbMigration
    {
        public override void Up()
        {
            AddForeignKey("DMS.DictionaryPositionExecutors", "AgentId", "DMS.DictionaryAgentEmployees", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("DMS.DictionaryPositionExecutors", "AgentId", "DMS.DictionaryAgentEmployees");
        }
    }
}
