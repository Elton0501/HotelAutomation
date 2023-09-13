namespace DataBase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Tax : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Restaurants", "Tax", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("dbo.Restaurants", "Service");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Restaurants", "Service", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("dbo.Restaurants", "Tax");
        }
    }
}
