namespace NUMLPay_Api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UsersDateTimeimplemented : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Users", "fp_token_expiry", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Users", "fp_token_expiry", c => c.DateTime(nullable: false));
        }
    }
}
