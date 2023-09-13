namespace DataBase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DiscountAppliedcouponEdit2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.BillDiscounts", "MinimumAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.BillDiscounts", "MinimumAmount", c => c.String());
        }
    }
}
