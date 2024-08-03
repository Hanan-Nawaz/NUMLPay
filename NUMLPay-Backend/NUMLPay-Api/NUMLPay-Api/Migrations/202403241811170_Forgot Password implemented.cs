namespace NUMLPay_Api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ForgotPasswordimplemented : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Admins", "fp_token", c => c.String());
            AddColumn("dbo.Admins", "fp_token_expiry", c => c.DateTime(nullable: false));
            AddColumn("dbo.Users", "is_ceased", c => c.Int(nullable: false));
            AddColumn("dbo.Users", "is_relegated", c => c.Int(nullable: false));
            AddColumn("dbo.Users", "passed_ceased_sems", c => c.Int(nullable: false));
            AddColumn("dbo.Users", "fp_token", c => c.String());
            AddColumn("dbo.Users", "fp_token_expiry", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "fp_token_expiry");
            DropColumn("dbo.Users", "fp_token");
            DropColumn("dbo.Users", "passed_ceased_sems");
            DropColumn("dbo.Users", "is_relegated");
            DropColumn("dbo.Users", "is_ceased");
            DropColumn("dbo.Admins", "fp_token_expiry");
            DropColumn("dbo.Admins", "fp_token");
        }
    }
}
