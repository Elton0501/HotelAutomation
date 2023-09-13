namespace DataBase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TaxApply : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Restaurants", "TaxApply", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Restaurants", "TaxApply");
        }
    }
}
