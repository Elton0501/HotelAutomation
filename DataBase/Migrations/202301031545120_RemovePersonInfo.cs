namespace DataBase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemovePersonInfo : DbMigration
    {
        public override void Up()
        {
            DropTable("dbo.PersonInformations");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.PersonInformations",
                c => new
                    {
                        PersonID = c.Int(nullable: false, identity: true),
                        PersonName = c.String(),
                        PersonPhoneNo = c.String(),
                        PersonAddress = c.String(),
                    })
                .PrimaryKey(t => t.PersonID);
            
        }
    }
}
