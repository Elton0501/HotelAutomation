namespace DataBase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TaxAmount : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "TaxAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Orders", "VatAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Orders", "VatAmount");
            DropColumn("dbo.Orders", "TaxAmount");
        }
    }
}
