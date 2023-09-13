namespace DataBase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Requiredfield : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Menus", "MDesc", c => c.String(nullable: false));
            AlterColumn("dbo.MenuCategories", "MCDesc", c => c.String(nullable: false));
            AlterColumn("dbo.MenuTypes", "MTDesc", c => c.String(nullable: false));
            AlterColumn("dbo.Packages", "PName", c => c.String(nullable: false));
            AlterColumn("dbo.Restaurants", "Name", c => c.String(nullable: false));
            AlterColumn("dbo.Restaurants", "Branch", c => c.String(nullable: false));
            AlterColumn("dbo.Restaurants", "City", c => c.String(nullable: false));
            AlterColumn("dbo.Restaurants", "Country", c => c.String(nullable: false));
            AlterColumn("dbo.Restaurants", "Url", c => c.String(nullable: false));
            AlterColumn("dbo.Restaurants", "Mobile", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Restaurants", "Mobile", c => c.String());
            AlterColumn("dbo.Restaurants", "Url", c => c.String());
            AlterColumn("dbo.Restaurants", "Country", c => c.String());
            AlterColumn("dbo.Restaurants", "City", c => c.String());
            AlterColumn("dbo.Restaurants", "Branch", c => c.String());
            AlterColumn("dbo.Restaurants", "Name", c => c.String());
            AlterColumn("dbo.Packages", "PName", c => c.String());
            AlterColumn("dbo.MenuTypes", "MTDesc", c => c.String());
            AlterColumn("dbo.MenuCategories", "MCDesc", c => c.String());
            AlterColumn("dbo.Menus", "MDesc", c => c.String());
        }
    }
}
