namespace DataBase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PersonInfo : DbMigration
    {
        public override void Up()
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
        
        public override void Down()
        {
            DropTable("dbo.PersonInformations");
        }
    }
}
