namespace NUMLPay_Api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Sessionisactiveadded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Sessions", "is_active", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Sessions", "is_active");
        }
    }
}
