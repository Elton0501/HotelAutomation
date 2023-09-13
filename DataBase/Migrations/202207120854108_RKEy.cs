namespace DataBase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RKEy : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Restaurants", "Key", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Restaurants", "Key");
        }
    }
}
