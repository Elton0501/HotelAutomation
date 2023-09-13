namespace DataBase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TCount1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Restaurants", "TableCount", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Restaurants", "TableCount", c => c.Int(nullable: false));
        }
    }
}
