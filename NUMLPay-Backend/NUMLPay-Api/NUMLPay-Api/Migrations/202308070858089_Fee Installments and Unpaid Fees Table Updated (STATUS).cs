namespace NUMLPay_Api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FeeInstallmentsandUnpaidFeesTableUpdatedSTATUS : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FeeInstallments", "status", c => c.Int(nullable: false));
            DropColumn("dbo.UnpaidFees", "status");
        }
        
        public override void Down()
        {
            AddColumn("dbo.UnpaidFees", "status", c => c.Int(nullable: false));
            DropColumn("dbo.FeeInstallments", "status");
        }
    }
}
