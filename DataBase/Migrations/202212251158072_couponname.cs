namespace DataBase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class couponname : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PlaceOrders", "CouponName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.PlaceOrders", "CouponName");
        }
    }
}
