namespace DMS_WebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AspNetUserContexts : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AspNetUserContexts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(),
                        Token = c.String(maxLength: 2000),
                        ClientId = c.Int(nullable: false),
                        CurrentPositionsIdList = c.String(maxLength: 2000),
                        DatabaseId = c.Int(nullable: false),
                        IsChangePasswordRequired = c.Boolean(nullable: false),
                        LoginLogId = c.Int(),
                        LoginLogInfo = c.String(maxLength: 2000),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.AspNetUserContexts");
        }
    }
}
