namespace DataBase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Inventory2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Inventories", "Name", c => c.String(nullable: false));
            AlterColumn("dbo.Inventories", "Unit", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Inventories", "Unit", c => c.String());
            AlterColumn("dbo.Inventories", "Name", c => c.String());
        }
    }
}
