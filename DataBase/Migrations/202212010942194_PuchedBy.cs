namespace DataBase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PuchedBy : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderItems", "PunchedBy", c => c.String());
            AddColumn("dbo.Orders", "PunchedBy", c => c.String());
            AddColumn("dbo.PlaceOrderItems", "PunchedBy", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.PlaceOrderItems", "PunchedBy");
            DropColumn("dbo.Orders", "PunchedBy");
            DropColumn("dbo.OrderItems", "PunchedBy");
        }
    }
}
