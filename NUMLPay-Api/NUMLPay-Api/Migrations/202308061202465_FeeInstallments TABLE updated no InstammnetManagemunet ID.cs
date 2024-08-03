namespace NUMLPay_Api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FeeInstallmentsTABLEupdatednoInstammnetManagemunetID : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.FeeInstallments", "installment_id", "dbo.InstallmentManagements");
            DropIndex("dbo.FeeInstallments", new[] { "installment_id" });
            DropColumn("dbo.FeeInstallments", "installment_id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.FeeInstallments", "installment_id", c => c.Int(nullable: false));
            CreateIndex("dbo.FeeInstallments", "installment_id");
            AddForeignKey("dbo.FeeInstallments", "installment_id", "dbo.InstallmentManagements", "installment_id", cascadeDelete: true);
        }
    }
}
