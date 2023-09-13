namespace DataBase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeCart : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CartItems", "IsNew", c => c.Boolean(nullable: false));
            AddColumn("dbo.CartItems", "IsOrderd", c => c.Boolean(nullable: false));
            AddColumn("dbo.CartItems", "isServed", c => c.Boolean(nullable: false));
            AddColumn("dbo.CartItems", "isPrint", c => c.Boolean(nullable: false));
            DropColumn("dbo.Carts", "IsNew");
            DropColumn("dbo.Carts", "IsOrderd");
            DropColumn("dbo.Carts", "isPrint");
            DropColumn("dbo.Carts", "isServed");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Carts", "isServed", c => c.Boolean(nullable: false));
            AddColumn("dbo.Carts", "isPrint", c => c.Boolean(nullable: false));
            AddColumn("dbo.Carts", "IsOrderd", c => c.Boolean(nullable: false));
            AddColumn("dbo.Carts", "IsNew", c => c.Boolean(nullable: false));
            DropColumn("dbo.CartItems", "isPrint");
            DropColumn("dbo.CartItems", "isServed");
            DropColumn("dbo.CartItems", "IsOrderd");
            DropColumn("dbo.CartItems", "IsNew");
        }
    }
}
