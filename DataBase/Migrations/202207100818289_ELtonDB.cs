namespace DataBase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ELtonDB : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EmailOTPs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OTP = c.Int(nullable: false),
                        UId = c.Int(nullable: false),
                        UserDetails = c.Guid(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Carts", "Table", c => c.String());
            AddColumn("dbo.Menus", "Rating", c => c.String());
            DropTable("dbo.MenuImages");
            DropTable("dbo.Rates");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Rates",
                c => new
                    {
                        RateCode = c.Int(nullable: false, identity: true),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MCode = c.Int(nullable: false),
                        RCode = c.Int(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.RateCode);
            
            CreateTable(
                "dbo.MenuImages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Image = c.String(),
                        MCode = c.Int(nullable: false),
                        RCode = c.Int(nullable: false),
                        MCCode = c.Int(),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            DropColumn("dbo.Menus", "Rating");
            DropColumn("dbo.Carts", "Table");
            DropTable("dbo.EmailOTPs");
        }
    }
}
