namespace DataBase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CartItemDisc : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CartItems", "Discount", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.CartItems", "Discount");
        }
    }
}
