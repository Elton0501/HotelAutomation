namespace DataBase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TCount : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Restaurants", "TableCount", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Restaurants", "TableCount");
        }
    }
}
