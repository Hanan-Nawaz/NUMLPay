namespace NUMLPay_Api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class shifttableis_activeadded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Shifts", "is_active", c => c.Int(nullable: false));
            DropColumn("dbo.Degrees", "is_active");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Degrees", "is_active", c => c.Int(nullable: false));
            DropColumn("dbo.Shifts", "is_active");
        }
    }
}
