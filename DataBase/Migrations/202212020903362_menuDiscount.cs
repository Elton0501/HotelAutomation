namespace DataBase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class menuDiscount : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Menus", "Discount", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Menus", "Discount");
        }
    }
}
