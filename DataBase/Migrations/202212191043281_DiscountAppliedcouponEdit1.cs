namespace DataBase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DiscountAppliedcouponEdit1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.BillDiscounts", "DiscountPercentage", c => c.String(nullable: false));
            AlterColumn("dbo.BillDiscounts", "DiscountAmount", c => c.String(nullable: false));
            AlterColumn("dbo.BillDiscounts", "CouponOnDiscount", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.BillDiscounts", "CouponOnDiscount", c => c.String());
            AlterColumn("dbo.BillDiscounts", "DiscountAmount", c => c.String());
            AlterColumn("dbo.BillDiscounts", "DiscountPercentage", c => c.String());
        }
    }
}
