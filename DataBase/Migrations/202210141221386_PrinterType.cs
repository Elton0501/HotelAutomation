namespace DataBase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PrinterType : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Restaurants", "PrinterType", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Restaurants", "PrinterType");
        }
    }
}
