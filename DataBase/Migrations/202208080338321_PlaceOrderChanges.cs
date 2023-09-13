namespace DataBase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PlaceOrderChanges : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.CartItems", "CartId", "dbo.Carts");
            DropIndex("dbo.CartItems", new[] { "CartId" });
            DropPrimaryKey("dbo.Carts");
            DropPrimaryKey("dbo.CartItems");
            CreateTable(
                "dbo.PlaceOrderItems",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        MCode = c.Int(nullable: false),
                        Quantity = c.Int(nullable: false),
                        price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PlaceOrderId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.PlaceOrders", t => t.PlaceOrderId, cascadeDelete: true)
                .Index(t => t.PlaceOrderId);
            
            CreateTable(
                "dbo.PlaceOrders",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Address = c.String(),
                        Comment = c.String(),
                        isServed = c.Boolean(nullable: false),
                        isPrint = c.Boolean(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AlterColumn("dbo.Carts", "Id", c => c.Long(nullable: false, identity: true));
            AlterColumn("dbo.CartItems", "Id", c => c.Long(nullable: false, identity: true));
            AlterColumn("dbo.CartItems", "CartId", c => c.Long(nullable: false));
            AddPrimaryKey("dbo.Carts", "Id");
            AddPrimaryKey("dbo.CartItems", "Id");
            CreateIndex("dbo.CartItems", "CartId");
            AddForeignKey("dbo.CartItems", "CartId", "dbo.Carts", "Id", cascadeDelete: true);
            DropColumn("dbo.Carts", "Comment");
            DropColumn("dbo.Carts", "Address");
            DropColumn("dbo.CartItems", "IsNew");
            DropColumn("dbo.CartItems", "IsOrderd");
            DropColumn("dbo.CartItems", "isServed");
            DropColumn("dbo.CartItems", "isPrint");
            DropTable("dbo.BOTs");
            DropTable("dbo.KOTs");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.KOTs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MCode = c.Int(nullable: false),
                        MDesc = c.String(),
                        Quantity = c.Int(nullable: false),
                        Complimentary = c.String(),
                        Comment = c.String(),
                        IsComplimentary = c.Boolean(nullable: false),
                        IsOld = c.Boolean(nullable: false),
                        RCode = c.Int(nullable: false),
                        PrinterCode = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.BOTs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MCode = c.Int(nullable: false),
                        RCode = c.Int(nullable: false),
                        PrinterCode = c.String(),
                        Quantity = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.CartItems", "isPrint", c => c.Boolean(nullable: false));
            AddColumn("dbo.CartItems", "isServed", c => c.Boolean(nullable: false));
            AddColumn("dbo.CartItems", "IsOrderd", c => c.Boolean(nullable: false));
            AddColumn("dbo.CartItems", "IsNew", c => c.Boolean(nullable: false));
            AddColumn("dbo.Carts", "Address", c => c.String());
            AddColumn("dbo.Carts", "Comment", c => c.String());
            DropForeignKey("dbo.CartItems", "CartId", "dbo.Carts");
            DropForeignKey("dbo.PlaceOrderItems", "PlaceOrderId", "dbo.PlaceOrders");
            DropIndex("dbo.PlaceOrderItems", new[] { "PlaceOrderId" });
            DropIndex("dbo.CartItems", new[] { "CartId" });
            DropPrimaryKey("dbo.CartItems");
            DropPrimaryKey("dbo.Carts");
            AlterColumn("dbo.CartItems", "CartId", c => c.Int(nullable: false));
            AlterColumn("dbo.CartItems", "Id", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.Carts", "Id", c => c.Int(nullable: false, identity: true));
            DropTable("dbo.PlaceOrders");
            DropTable("dbo.PlaceOrderItems");
            AddPrimaryKey("dbo.CartItems", "Id");
            AddPrimaryKey("dbo.Carts", "Id");
            CreateIndex("dbo.CartItems", "CartId");
            AddForeignKey("dbo.CartItems", "CartId", "dbo.Carts", "Id", cascadeDelete: true);
        }
    }
}
