namespace DataBase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class forType : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BillDiscounts", "forBar", c => c.Boolean(nullable: false));
            AddColumn("dbo.BillDiscounts", "forFoodOrBevrage", c => c.Boolean(nullable: false));
            AddColumn("dbo.BillDiscounts", "ExpiredDateTime", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.BillDiscounts", "ExpiredDateTime");
            DropColumn("dbo.BillDiscounts", "forFoodOrBevrage");
            DropColumn("dbo.BillDiscounts", "forBar");
        }
    }
}
