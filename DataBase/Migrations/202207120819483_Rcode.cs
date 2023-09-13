namespace DataBase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Rcode : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Restaurants", "Email", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Restaurants", "Email");
        }
    }
}
