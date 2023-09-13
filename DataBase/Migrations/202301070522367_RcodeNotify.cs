namespace DataBase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RcodeNotify : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Notifications", "RCode", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Notifications", "RCode");
        }
    }
}
