namespace NUMLPay_Api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FeeInstallmentstableupdatedandFKaddedUnPaidFees : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.UnpaidFees", "fee_installments", "dbo.FeeInstallments");
            DropIndex("dbo.UnpaidFees", new[] { "fee_installments" });
            AddColumn("dbo.FeeInstallments", "challan_id", c => c.Int(nullable: false));
            CreateIndex("dbo.FeeInstallments", "challan_id");
            AddForeignKey("dbo.FeeInstallments", "challan_id", "dbo.UnpaidFees", "challan_no", cascadeDelete: true);
            DropColumn("dbo.UnpaidFees", "fee_installments");
        }
        
        public override void Down()
        {
            AddColumn("dbo.UnpaidFees", "fee_installments", c => c.Int(nullable: false));
            DropForeignKey("dbo.FeeInstallments", "challan_id", "dbo.UnpaidFees");
            DropIndex("dbo.FeeInstallments", new[] { "challan_id" });
            DropColumn("dbo.FeeInstallments", "challan_id");
            CreateIndex("dbo.UnpaidFees", "fee_installments");
            AddForeignKey("dbo.UnpaidFees", "fee_installments", "dbo.FeeInstallments", "id", cascadeDelete: true);
        }
    }
}
