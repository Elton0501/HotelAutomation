namespace DataBase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Changes2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Restaurants", "KitPrinter", c => c.String());
            AddColumn("dbo.Restaurants", "BevPrinter", c => c.String());
            AddColumn("dbo.Restaurants", "BillPrinter", c => c.String());
            AddColumn("dbo.Restaurants", "BarPrinter", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Restaurants", "BarPrinter");
            DropColumn("dbo.Restaurants", "BillPrinter");
            DropColumn("dbo.Restaurants", "BevPrinter");
            DropColumn("dbo.Restaurants", "KitPrinter");
        }
    }
}
