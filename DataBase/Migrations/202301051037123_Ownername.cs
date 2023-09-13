namespace DataBase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Ownername : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Restaurants", "OwnerName", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Restaurants", "OwnerName");
        }
    }
}
