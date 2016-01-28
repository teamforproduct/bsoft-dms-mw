namespace DMS_WebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class editfieldAgentIdAspNetUser : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.AspNetUsers", "AgentId", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.AspNetUsers", "AgentId", c => c.Int(nullable: false));
        }
    }
}
