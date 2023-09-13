namespace DataBase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class userui : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Restaurants", "ButtonPrimary", c => c.String());
            AddColumn("dbo.Restaurants", "ButtonSecondary", c => c.String());
            AddColumn("dbo.Restaurants", "ButtonPrimaryFont", c => c.String());
            AddColumn("dbo.Restaurants", "ButtonSecondaryFont", c => c.String());
            AddColumn("dbo.Restaurants", "Heading", c => c.String());
            AddColumn("dbo.Restaurants", "Bgblur", c => c.String());
            DropColumn("dbo.Restaurants", "Button");
            DropColumn("dbo.Restaurants", "ButtonFont");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Restaurants", "ButtonFont", c => c.String());
            AddColumn("dbo.Restaurants", "Button", c => c.String());
            DropColumn("dbo.Restaurants", "Bgblur");
            DropColumn("dbo.Restaurants", "Heading");
            DropColumn("dbo.Restaurants", "ButtonSecondaryFont");
            DropColumn("dbo.Restaurants", "ButtonPrimaryFont");
            DropColumn("dbo.Restaurants", "ButtonSecondary");
            DropColumn("dbo.Restaurants", "ButtonPrimary");
        }
    }
}
