namespace NUMLPay_Api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MFeetableupdatedis_activeAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MiscellaneousFees", "is_active", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.MiscellaneousFees", "is_active");
        }
    }
}
