namespace NUMLPay_Api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FeeInstallmenstandUnpaidTablesandAccountBookmTabkeupdated : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.UnpaidFees", "verified_by", "dbo.Admins");
            DropForeignKey("dbo.AccountBooks", "challan_no", "dbo.UnpaidFees");
            DropIndex("dbo.UnpaidFees", new[] { "verified_by" });
            AddColumn("dbo.FeeInstallments", "payment_method", c => c.Int(nullable: false));
            AddColumn("dbo.FeeInstallments", "image", c => c.String(maxLength: 8000, unicode: false));
            AddColumn("dbo.FeeInstallments", "verified_by", c => c.String(maxLength: 30, unicode: false));
            CreateIndex("dbo.FeeInstallments", "verified_by");
            AddForeignKey("dbo.FeeInstallments", "verified_by", "dbo.Admins", "email_id");
            AddForeignKey("dbo.AccountBooks", "challan_no", "dbo.FeeInstallments", "id", cascadeDelete: true);
            DropColumn("dbo.AccountBooks", "paid_date");
            DropColumn("dbo.UnpaidFees", "payment_method");
            DropColumn("dbo.UnpaidFees", "image");
            DropColumn("dbo.UnpaidFees", "paid_date");
            DropColumn("dbo.UnpaidFees", "verified_by");
        }
        
        public override void Down()
        {
            AddColumn("dbo.UnpaidFees", "verified_by", c => c.String(maxLength: 30, unicode: false));
            AddColumn("dbo.UnpaidFees", "paid_date", c => c.String());
            AddColumn("dbo.UnpaidFees", "image", c => c.String(maxLength: 8000, unicode: false));
            AddColumn("dbo.UnpaidFees", "payment_method", c => c.Int(nullable: false));
            AddColumn("dbo.AccountBooks", "paid_date", c => c.DateTime(nullable: false, storeType: "date"));
            DropForeignKey("dbo.AccountBooks", "challan_no", "dbo.FeeInstallments");
            DropForeignKey("dbo.FeeInstallments", "verified_by", "dbo.Admins");
            DropIndex("dbo.FeeInstallments", new[] { "verified_by" });
            DropColumn("dbo.FeeInstallments", "verified_by");
            DropColumn("dbo.FeeInstallments", "image");
            DropColumn("dbo.FeeInstallments", "payment_method");
            CreateIndex("dbo.UnpaidFees", "verified_by");
            AddForeignKey("dbo.AccountBooks", "challan_no", "dbo.UnpaidFees", "challan_no", cascadeDelete: true);
            AddForeignKey("dbo.UnpaidFees", "verified_by", "dbo.Admins", "email_id");
        }
    }
}
