namespace DataBase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DiscountAppliedcouponEdit : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BillDiscounts", "isSingleUse", c => c.Boolean(nullable: false));
            AddColumn("dbo.BillDiscounts", "HomeDelivery", c => c.Boolean(nullable: false));
            AddColumn("dbo.BillDiscounts", "TakeAway", c => c.Boolean(nullable: false));
            AddColumn("dbo.BillDiscounts", "Table", c => c.Boolean(nullable: false));
            DropColumn("dbo.BillDiscounts", "DiscountFor");
        }
        
        public override void Down()
        {
            AddColumn("dbo.BillDiscounts", "DiscountFor", c => c.String(nullable: false));
            DropColumn("dbo.BillDiscounts", "Table");
            DropColumn("dbo.BillDiscounts", "TakeAway");
            DropColumn("dbo.BillDiscounts", "HomeDelivery");
            DropColumn("dbo.BillDiscounts", "isSingleUse");
        }
    }
}
