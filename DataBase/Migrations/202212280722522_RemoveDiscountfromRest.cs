namespace DataBase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveDiscountfromRest : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Restaurants", "DiscountUpto");
            DropColumn("dbo.Restaurants", "Discount");
            DropColumn("dbo.Restaurants", "TADiscount");
            DropColumn("dbo.Restaurants", "HDDiscount");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Restaurants", "HDDiscount", c => c.String());
            AddColumn("dbo.Restaurants", "TADiscount", c => c.String());
            AddColumn("dbo.Restaurants", "Discount", c => c.String());
            AddColumn("dbo.Restaurants", "DiscountUpto", c => c.Decimal(precision: 18, scale: 2));
        }
    }
}
