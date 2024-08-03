namespace NUMLPay_Api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AdminandUsersTableUpdatedSaltRemoved : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Admins", "Salt");
            DropColumn("dbo.Users", "Salt");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Users", "Salt", c => c.String(nullable: false, maxLength: 500, unicode: false));
            AddColumn("dbo.Admins", "Salt", c => c.String(nullable: false, maxLength: 500, unicode: false));
        }
    }
}
