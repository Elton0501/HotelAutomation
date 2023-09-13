namespace DataBase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LongOItem : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.OrderItems");
            AlterColumn("dbo.OrderItems", "Id", c => c.Long(nullable: false, identity: true));
            AddPrimaryKey("dbo.OrderItems", "Id");
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.OrderItems");
            AlterColumn("dbo.OrderItems", "Id", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.OrderItems", "Id");
        }
    }
}
