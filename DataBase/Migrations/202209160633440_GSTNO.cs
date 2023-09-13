namespace DataBase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class GSTNO : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Restaurants", "ReviewUrl", c => c.String());
            AddColumn("dbo.Restaurants", "GSTIN", c => c.String());
            AddColumn("dbo.Restaurants", "FASSAI", c => c.String());
            AlterColumn("dbo.Restaurants", "Url", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Restaurants", "Url", c => c.String(nullable: false));
            DropColumn("dbo.Restaurants", "FASSAI");
            DropColumn("dbo.Restaurants", "GSTIN");
            DropColumn("dbo.Restaurants", "ReviewUrl");
        }
    }
}
