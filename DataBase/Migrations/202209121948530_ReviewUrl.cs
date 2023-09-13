namespace DataBase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ReviewUrl : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "PaymentType", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Orders", "PaymentType");
        }
    }
}
