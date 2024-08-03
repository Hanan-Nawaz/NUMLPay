namespace NUMLPay_Api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PasswordSizeincrease : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Admins", "password", c => c.String(nullable: false, maxLength: 1000, unicode: false));
            AlterColumn("dbo.Users", "password", c => c.String(nullable: false, maxLength: 1000, unicode: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Users", "password", c => c.String(nullable: false, maxLength: 15, unicode: false));
            AlterColumn("dbo.Admins", "password", c => c.String(nullable: false, maxLength: 15, unicode: false));
        }
    }
}
