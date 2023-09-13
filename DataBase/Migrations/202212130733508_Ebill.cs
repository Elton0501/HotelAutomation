namespace DataBase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Ebill : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "isEbill", c => c.Boolean(nullable: false));
            AddColumn("dbo.PlaceOrders", "isEbill", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PlaceOrders", "isEbill");
            DropColumn("dbo.Orders", "isEbill");
        }
    }
}
