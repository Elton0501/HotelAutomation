namespace DataBase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DiscountAppliedcoupon : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CouponApplieds",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RCode = c.String(),
                        UserNumber = c.String(),
                        UseEmail = c.String(),
                        CouponName = c.String(),
                        AppliedOn = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.BillDiscounts", "CouponName", c => c.String());
            AddColumn("dbo.BillDiscounts", "MinimumAmount", c => c.String());
            AddColumn("dbo.BillDiscounts", "CouponOnDiscount", c => c.String());
            AddColumn("dbo.BillDiscounts", "CreatedOn", c => c.DateTime(nullable: false));
            AddColumn("dbo.BillDiscounts", "CreatedBy", c => c.String());
            AddColumn("dbo.BillDiscounts", "IsActive", c => c.Boolean(nullable: false));
            DropColumn("dbo.BillDiscounts", "DiscountUpto");
        }
        
        public override void Down()
        {
            AddColumn("dbo.BillDiscounts", "DiscountUpto", c => c.String());
            DropColumn("dbo.BillDiscounts", "IsActive");
            DropColumn("dbo.BillDiscounts", "CreatedBy");
            DropColumn("dbo.BillDiscounts", "CreatedOn");
            DropColumn("dbo.BillDiscounts", "CouponOnDiscount");
            DropColumn("dbo.BillDiscounts", "MinimumAmount");
            DropColumn("dbo.BillDiscounts", "CouponName");
            DropTable("dbo.CouponApplieds");
        }
    }
}
