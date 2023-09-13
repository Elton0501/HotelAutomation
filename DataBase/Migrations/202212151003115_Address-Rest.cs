namespace DataBase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddressRest : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Restaurants", "Address", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Restaurants", "Address");
        }
    }
}
