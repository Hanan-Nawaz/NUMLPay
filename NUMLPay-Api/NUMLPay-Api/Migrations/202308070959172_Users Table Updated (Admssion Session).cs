namespace NUMLPay_Api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UsersTableUpdatedAdmssionSession : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Users", "admission_session", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Users", "admission_session", c => c.String(nullable: false, maxLength: 20, unicode: false));
        }
    }
}
