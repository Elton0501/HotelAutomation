namespace DataBase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DiscountItem : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BillDiscounts", "DiscountOnItem", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.BillDiscounts", "DiscountOnItem");
        }
    }
}
