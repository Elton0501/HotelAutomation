namespace DataBase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PlaceOrderChanges72219 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PlaceOrders", "RCode", c => c.Int(nullable: false));
            AddColumn("dbo.PlaceOrders", "BillPayed", c => c.Boolean(nullable: false));
            AddColumn("dbo.PlaceOrders", "Table", c => c.String());
            DropColumn("dbo.Carts", "BillPayed");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Carts", "BillPayed", c => c.Boolean(nullable: false));
            DropColumn("dbo.PlaceOrders", "Table");
            DropColumn("dbo.PlaceOrders", "BillPayed");
            DropColumn("dbo.PlaceOrders", "RCode");
        }
    }
}
