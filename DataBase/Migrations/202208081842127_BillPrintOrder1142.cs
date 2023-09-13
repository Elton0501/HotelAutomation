namespace DataBase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BillPrintOrder1142 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BillPrintOrders", "OrderId", c => c.Long(nullable: false));
            AddColumn("dbo.BillPrintOrders", "isBill", c => c.Boolean(nullable: false));
            DropColumn("dbo.BillPrintOrders", "PlaceOrderId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.BillPrintOrders", "PlaceOrderId", c => c.Int(nullable: false));
            DropColumn("dbo.BillPrintOrders", "isBill");
            DropColumn("dbo.BillPrintOrders", "OrderId");
        }
    }
}
