namespace NUMLPay_Api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChallanVerificationTableAdded : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.FeeInstallments", "verified_by", "dbo.Admins");
            DropIndex("dbo.FeeInstallments", new[] { "verified_by" });
            CreateTable(
                "dbo.ChallanVerifications",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        fee_installment_id = c.Int(nullable: false),
                        image = c.String(maxLength: 8000, unicode: false),
                        verified_by = c.String(maxLength: 30, unicode: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Admins", t => t.verified_by)
                .ForeignKey("dbo.FeeInstallments", t => t.fee_installment_id, cascadeDelete: true)
                .Index(t => t.fee_installment_id)
                .Index(t => t.verified_by);
            
            DropColumn("dbo.FeeInstallments", "image");
            DropColumn("dbo.FeeInstallments", "verified_by");
        }
        
        public override void Down()
        {
            AddColumn("dbo.FeeInstallments", "verified_by", c => c.String(maxLength: 30, unicode: false));
            AddColumn("dbo.FeeInstallments", "image", c => c.String(maxLength: 8000, unicode: false));
            DropForeignKey("dbo.ChallanVerifications", "fee_installment_id", "dbo.FeeInstallments");
            DropForeignKey("dbo.ChallanVerifications", "verified_by", "dbo.Admins");
            DropIndex("dbo.ChallanVerifications", new[] { "verified_by" });
            DropIndex("dbo.ChallanVerifications", new[] { "fee_installment_id" });
            DropTable("dbo.ChallanVerifications");
            CreateIndex("dbo.FeeInstallments", "verified_by");
            AddForeignKey("dbo.FeeInstallments", "verified_by", "dbo.Admins", "email_id");
        }
    }
}
