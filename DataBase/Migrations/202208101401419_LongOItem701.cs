namespace DataBase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LongOItem701 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.BillPrintOrders", "OrderId", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.BillPrintOrders", "OrderId", c => c.Long(nullable: false));
        }
    }
}
