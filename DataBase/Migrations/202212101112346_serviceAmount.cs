namespace DataBase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class serviceAmount : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "ServiceAmount", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.Restaurants", "ServiceTax", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.Orders", "VatAmount", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Orders", "VatAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("dbo.Restaurants", "ServiceTax");
            DropColumn("dbo.Orders", "ServiceAmount");
        }
    }
}
