namespace DataBase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Events : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Restaurants", "YouandMe", c => c.Boolean(nullable: false));
            AddColumn("dbo.Restaurants", "BirthdayParty", c => c.Boolean(nullable: false));
            AddColumn("dbo.Restaurants", "FarewellParty", c => c.Boolean(nullable: false));
            AddColumn("dbo.Restaurants", "FamilyDinner", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Restaurants", "FamilyDinner");
            DropColumn("dbo.Restaurants", "FarewellParty");
            DropColumn("dbo.Restaurants", "BirthdayParty");
            DropColumn("dbo.Restaurants", "YouandMe");
        }
    }
}
