namespace DataBase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CartItems : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CartItems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MCode = c.Int(nullable: false),
                        Quantity = c.Int(nullable: false),
                        price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CartId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Carts", t => t.CartId, cascadeDelete: true)
                .Index(t => t.CartId);
            
            AddColumn("dbo.Carts", "isPrint", c => c.Boolean(nullable: false));
            AddColumn("dbo.Carts", "isServed", c => c.Boolean(nullable: false));
            AddColumn("dbo.Carts", "CreatedOn", c => c.DateTime(nullable: false));
            AddColumn("dbo.Carts", "CreatedBy", c => c.String());
            AddColumn("dbo.Carts", "IsActive", c => c.Boolean(nullable: false));
            DropColumn("dbo.Carts", "MCode");
            DropColumn("dbo.Carts", "UserIdentity");
            DropColumn("dbo.Carts", "Quantity");
            DropColumn("dbo.Carts", "OrderDateTime");
            DropColumn("dbo.Carts", "price");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Carts", "price", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Carts", "OrderDateTime", c => c.String());
            AddColumn("dbo.Carts", "Quantity", c => c.Int(nullable: false));
            AddColumn("dbo.Carts", "UserIdentity", c => c.String());
            AddColumn("dbo.Carts", "MCode", c => c.Int(nullable: false));
            DropForeignKey("dbo.CartItems", "CartId", "dbo.Carts");
            DropIndex("dbo.CartItems", new[] { "CartId" });
            DropColumn("dbo.Carts", "IsActive");
            DropColumn("dbo.Carts", "CreatedBy");
            DropColumn("dbo.Carts", "CreatedOn");
            DropColumn("dbo.Carts", "isServed");
            DropColumn("dbo.Carts", "isPrint");
            DropTable("dbo.CartItems");
        }
    }
}
