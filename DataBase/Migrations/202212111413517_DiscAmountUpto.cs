namespace DataBase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DiscAmountUpto : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Restaurants", "DiscountUpto", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Restaurants", "DiscountUpto");
        }
    }
}
