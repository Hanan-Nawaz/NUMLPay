namespace NUMLPay_Api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SummerManagementUpdatedInactiveadded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Subjects", "is_active", c => c.Int(nullable: false));
            AddColumn("dbo.SummerEnrollments", "is_active", c => c.Int(nullable: false));
            AddColumn("dbo.SummerFees", "is_active", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SummerFees", "is_active");
            DropColumn("dbo.SummerEnrollments", "is_active");
            DropColumn("dbo.Subjects", "is_active");
        }
    }
}
