namespace NUMLPay_Api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PasswordAddedtoUsersTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "password", c => c.String(nullable: false, maxLength: 15, unicode: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "password");
        }
    }
}
