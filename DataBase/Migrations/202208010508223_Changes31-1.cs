namespace DataBase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Changes311 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Carts", "Address", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Carts", "Address");
        }
    }
}
