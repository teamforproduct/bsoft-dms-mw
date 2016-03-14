namespace BL.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DictionarySubscriptionStates : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DictionarySubscriptionStates",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(),
                        Name = c.String(),
                        IsSuccess = c.Boolean(nullable: false),
                        LastChangeUserId = c.Int(nullable: false),
                        LastChangeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.DocumentSubscriptions", "SubscriptionStateId", c => c.Int());
            CreateIndex("dbo.DocumentSubscriptions", "SubscriptionStateId");
            AddForeignKey("dbo.DocumentSubscriptions", "SubscriptionStateId", "dbo.DictionarySubscriptionStates", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DocumentSubscriptions", "SubscriptionStateId", "dbo.DictionarySubscriptionStates");
            DropIndex("dbo.DocumentSubscriptions", new[] { "SubscriptionStateId" });
            DropColumn("dbo.DocumentSubscriptions", "SubscriptionStateId");
            DropTable("dbo.DictionarySubscriptionStates");
        }
    }
}
