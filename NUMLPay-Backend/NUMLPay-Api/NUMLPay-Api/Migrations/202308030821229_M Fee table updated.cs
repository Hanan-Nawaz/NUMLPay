namespace NUMLPay_Api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MFeetableupdated : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.MiscellaneousFees", "session");
        }
        
        public override void Down()
        {
            AddColumn("dbo.MiscellaneousFees", "session", c => c.Int(nullable: false));
        }
    }
}
