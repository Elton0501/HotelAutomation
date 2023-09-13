namespace DataBase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PlaceOrderChanges72234 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PlaceOrders", "isOld", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PlaceOrders", "isOld");
        }
    }
}
