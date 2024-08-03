namespace NUMLPay_Api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UnpaidFeesTableUpdatedIssueDate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UnpaidFees", "issue_date", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.UnpaidFees", "issue_date");
        }
    }
}
