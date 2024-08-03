namespace NUMLPay_Api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UsersTableupdatedsomeColumnRequiredStatusChnaged : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Users", "father_name", c => c.String(maxLength: 50, unicode: false));
            AlterColumn("dbo.Users", "email", c => c.String(maxLength: 50, unicode: false));
            AlterColumn("dbo.Users", "contact", c => c.String(maxLength: 50, unicode: false));
            AlterColumn("dbo.Users", "nic", c => c.String(maxLength: 15, unicode: false));
            AlterColumn("dbo.Users", "image", c => c.String(maxLength: 8000, unicode: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Users", "image", c => c.String(nullable: false, maxLength: 8000, unicode: false));
            AlterColumn("dbo.Users", "nic", c => c.String(nullable: false, maxLength: 15, unicode: false));
            AlterColumn("dbo.Users", "contact", c => c.String(nullable: false, maxLength: 50, unicode: false));
            AlterColumn("dbo.Users", "email", c => c.String(nullable: false, maxLength: 50, unicode: false));
            AlterColumn("dbo.Users", "father_name", c => c.String(nullable: false, maxLength: 50, unicode: false));
        }
    }
}
