namespace NUMLPay_Api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FeeGenerationChnagesinUnpaidFeesTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UnpaidFees", "fee_instalments_id", c => c.Int());
            AddColumn("dbo.UnpaidFees", "fee_type", c => c.Int(nullable: false));
            CreateIndex("dbo.UnpaidFees", "fee_instalments_id");
            AddForeignKey("dbo.UnpaidFees", "fee_instalments_id", "dbo.InstallmentManagements", "installment_id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UnpaidFees", "fee_instalments_id", "dbo.InstallmentManagements");
            DropIndex("dbo.UnpaidFees", new[] { "fee_instalments_id" });
            DropColumn("dbo.UnpaidFees", "fee_type");
            DropColumn("dbo.UnpaidFees", "fee_instalments_id");
        }
    }
}
