namespace DataBase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Changes30 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Restaurants", "KitPrinter");
            DropColumn("dbo.Restaurants", "BevPrinter");
            DropColumn("dbo.Restaurants", "BillPrinter");
            DropColumn("dbo.Restaurants", "BarPrinter");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Restaurants", "BarPrinter", c => c.String());
            AddColumn("dbo.Restaurants", "BillPrinter", c => c.String());
            AddColumn("dbo.Restaurants", "BevPrinter", c => c.String());
            AddColumn("dbo.Restaurants", "KitPrinter", c => c.String());
        }
    }
}
