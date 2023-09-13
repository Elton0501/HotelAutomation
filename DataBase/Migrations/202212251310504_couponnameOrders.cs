namespace DataBase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class couponnameOrders : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "CouponAmount", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.Orders", "CouponDiscount", c => c.String());
            AddColumn("dbo.Orders", "CouponName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Orders", "CouponName");
            DropColumn("dbo.Orders", "CouponDiscount");
            DropColumn("dbo.Orders", "CouponAmount");
        }
    }
}
