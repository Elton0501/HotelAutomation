namespace DataBase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DiscountOrder : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "Discount", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Orders", "Discount");
        }
    }
}
