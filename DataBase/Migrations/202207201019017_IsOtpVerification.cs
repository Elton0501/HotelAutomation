namespace DataBase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IsOtpVerification : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Restaurants", "IsOtpVerification", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Restaurants", "IsOtpVerification");
        }
    }
}
