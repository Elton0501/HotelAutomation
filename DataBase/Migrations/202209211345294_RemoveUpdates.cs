namespace DataBase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveUpdates : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Restaurants", "Recommended");
            DropColumn("dbo.Restaurants", "Trending");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Restaurants", "Trending", c => c.Boolean(nullable: false));
            AddColumn("dbo.Restaurants", "Recommended", c => c.Boolean(nullable: false));
        }
    }
}
