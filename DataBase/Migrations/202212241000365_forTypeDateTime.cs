namespace DataBase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class forTypeDateTime : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.BillDiscounts", "ExpiredDateTime", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.BillDiscounts", "ExpiredDateTime", c => c.String());
        }
    }
}
