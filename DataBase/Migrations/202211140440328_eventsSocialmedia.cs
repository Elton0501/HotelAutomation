namespace DataBase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class eventsSocialmedia : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Restaurants", "RestDesc", c => c.String());
            AddColumn("dbo.Restaurants", "Youtube", c => c.String());
            AddColumn("dbo.Restaurants", "Facebook", c => c.String());
            AddColumn("dbo.Restaurants", "Instagram", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Restaurants", "Instagram");
            DropColumn("dbo.Restaurants", "Facebook");
            DropColumn("dbo.Restaurants", "Youtube");
            DropColumn("dbo.Restaurants", "RestDesc");
        }
    }
}
