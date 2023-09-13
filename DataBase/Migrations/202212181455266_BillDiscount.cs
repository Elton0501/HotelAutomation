namespace DataBase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BillDiscount : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BillDiscounts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RCode = c.String(nullable: false),
                        DiscountFor = c.String(nullable: false),
                        DiscountPercentage = c.String(),
                        DiscountAmount = c.String(),
                        DiscountUpto = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.BillDiscounts");
        }
    }
}
