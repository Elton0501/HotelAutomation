namespace DataBase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AllDiscounts : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Menus", "TADiscount", c => c.String());
            AddColumn("dbo.Menus", "HDDiscount", c => c.String());
            AddColumn("dbo.Restaurants", "Discount", c => c.String());
            AddColumn("dbo.Restaurants", "TADiscount", c => c.String());
            AddColumn("dbo.Restaurants", "HDDiscount", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Restaurants", "HDDiscount");
            DropColumn("dbo.Restaurants", "TADiscount");
            DropColumn("dbo.Restaurants", "Discount");
            DropColumn("dbo.Menus", "HDDiscount");
            DropColumn("dbo.Menus", "TADiscount");
        }
    }
}
