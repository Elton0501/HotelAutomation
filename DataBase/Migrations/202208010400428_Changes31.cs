namespace DataBase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Changes31 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Carts", "Comment", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Carts", "Comment");
        }
    }
}
