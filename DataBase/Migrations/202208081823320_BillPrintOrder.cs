namespace DataBase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BillPrintOrder : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BillPrintOrders",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PlaceOrderId = c.Int(nullable: false),
                        RCode = c.Int(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.BillPrintOrders");
        }
    }
}
