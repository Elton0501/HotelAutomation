namespace DataBase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DiffAmount : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "FoodTotalAmount", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.Orders", "BarTotalAmount", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.Orders", "BevrageTotalAmount", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Orders", "BevrageTotalAmount");
            DropColumn("dbo.Orders", "BarTotalAmount");
            DropColumn("dbo.Orders", "FoodTotalAmount");
        }
    }
}
