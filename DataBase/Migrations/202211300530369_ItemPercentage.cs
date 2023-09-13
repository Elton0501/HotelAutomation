namespace DataBase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ItemPercentage : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderItems", "percentage", c => c.String());
            AddColumn("dbo.PlaceOrderItems", "percentage", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.PlaceOrderItems", "percentage");
            DropColumn("dbo.OrderItems", "percentage");
        }
    }
}
