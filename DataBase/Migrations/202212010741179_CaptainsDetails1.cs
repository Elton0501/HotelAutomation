namespace DataBase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CaptainsDetails1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CaptainDetails", "RCode", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.CaptainDetails", "RCode");
        }
    }
}
