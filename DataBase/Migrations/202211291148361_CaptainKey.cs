namespace DataBase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CaptainKey : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Restaurants", "CaptainKey", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Restaurants", "CaptainKey");
        }
    }
}
