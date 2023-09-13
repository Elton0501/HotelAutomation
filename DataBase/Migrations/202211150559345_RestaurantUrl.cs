namespace DataBase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RestaurantUrl : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Restaurants", "RestaurantUrl", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Restaurants", "RestaurantUrl");
        }
    }
}
