namespace DataBase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OrderTime : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Carts", "OrderDateTime", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Carts", "OrderDateTime");
        }
    }
}
