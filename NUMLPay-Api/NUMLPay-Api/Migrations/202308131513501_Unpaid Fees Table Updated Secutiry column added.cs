namespace NUMLPay_Api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UnpaidFeesTableUpdatedSecutirycolumnadded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UnpaidFees", "security", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.UnpaidFees", "security");
        }
    }
}
