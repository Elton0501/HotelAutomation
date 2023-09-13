namespace DataBase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Inventory1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Inventories", "Status", c => c.Boolean(nullable: false));
            DropColumn("dbo.Inventories", "FullInStock");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Inventories", "FullInStock", c => c.Int(nullable: false));
            DropColumn("dbo.Inventories", "Status");
        }
    }
}
