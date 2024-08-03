namespace NUMLPay_Api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AdminandUsersTableUpdatedSaltAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Admins", "Salt", c => c.String(nullable: false, maxLength: 500, unicode: false));
            AddColumn("dbo.Users", "Salt", c => c.String(nullable: false, maxLength: 500, unicode: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "Salt");
            DropColumn("dbo.Admins", "Salt");
        }
    }
}
