namespace NUMLPay_Api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChallansandUnpaidTablesUpdated : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.UnpaidFees", "fee_id", "dbo.FeeStructures");
            DropForeignKey("dbo.UnpaidFees", "challan_id", "dbo.Challans");
            DropIndex("dbo.UnpaidFees", new[] { "challan_id" });
            DropIndex("dbo.UnpaidFees", new[] { "fee_id" });
            AddColumn("dbo.Challans", "session", c => c.Int(nullable: false));
            AddColumn("dbo.Challans", "valid_date", c => c.DateTime(nullable: false, storeType: "date"));
            AddColumn("dbo.Challans", "fine_id", c => c.Int(nullable: false));
            AlterColumn("dbo.UnpaidFees", "challan_id", c => c.Int());
            CreateIndex("dbo.UnpaidFees", "challan_id");
            CreateIndex("dbo.Challans", "fine_id");
            AddForeignKey("dbo.Challans", "fine_id", "dbo.Fines", "id", cascadeDelete: true);
            AddForeignKey("dbo.UnpaidFees", "challan_id", "dbo.Challans", "challan_id");
            DropColumn("dbo.Challans", "current_session");
            DropColumn("dbo.Challans", "issue_date");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Challans", "issue_date", c => c.DateTime(nullable: false, storeType: "date"));
            AddColumn("dbo.Challans", "current_session", c => c.String(nullable: false, maxLength: 50, unicode: false));
            DropForeignKey("dbo.UnpaidFees", "challan_id", "dbo.Challans");
            DropForeignKey("dbo.Challans", "fine_id", "dbo.Fines");
            DropIndex("dbo.Challans", new[] { "fine_id" });
            DropIndex("dbo.UnpaidFees", new[] { "challan_id" });
            AlterColumn("dbo.UnpaidFees", "challan_id", c => c.Int(nullable: false));
            DropColumn("dbo.Challans", "fine_id");
            DropColumn("dbo.Challans", "valid_date");
            DropColumn("dbo.Challans", "session");
            CreateIndex("dbo.UnpaidFees", "fee_id");
            CreateIndex("dbo.UnpaidFees", "challan_id");
            AddForeignKey("dbo.UnpaidFees", "challan_id", "dbo.Challans", "challan_id", cascadeDelete: true);
            AddForeignKey("dbo.UnpaidFees", "fee_id", "dbo.FeeStructures", "Id", cascadeDelete: true);
        }
    }
}
