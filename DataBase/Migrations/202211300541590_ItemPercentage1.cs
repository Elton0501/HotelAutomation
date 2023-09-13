namespace DataBase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ItemPercentage1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderItems", "Discount", c => c.String());
            AddColumn("dbo.PlaceOrderItems", "Discount", c => c.String());
            DropColumn("dbo.OrderItems", "percentage");
            DropColumn("dbo.PlaceOrderItems", "percentage");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PlaceOrderItems", "percentage", c => c.String());
            AddColumn("dbo.OrderItems", "percentage", c => c.String());
            DropColumn("dbo.PlaceOrderItems", "Discount");
            DropColumn("dbo.OrderItems", "Discount");
        }
    }
}
