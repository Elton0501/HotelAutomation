namespace DataBase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserAddress : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "UserAddress", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Orders", "UserAddress");
        }
    }
}
