namespace NUMLPay_Api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Check : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FeePlans", "is_active", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.FeePlans", "is_active");
        }
    }
}
