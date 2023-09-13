namespace DataBase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ISPLACEORDERID : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BillPrintOrders", "isPlaceOrder", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.BillPrintOrders", "isPlaceOrder");
        }
    }
}
