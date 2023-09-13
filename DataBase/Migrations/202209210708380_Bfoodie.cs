namespace DataBase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Bfoodie : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Restaurants", "RestImg", c => c.String());
            AddColumn("dbo.Restaurants", "Recommended", c => c.Boolean(nullable: false));
            AddColumn("dbo.Restaurants", "Trending", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Restaurants", "Trending");
            DropColumn("dbo.Restaurants", "Recommended");
            DropColumn("dbo.Restaurants", "RestImg");
        }
    }
}
