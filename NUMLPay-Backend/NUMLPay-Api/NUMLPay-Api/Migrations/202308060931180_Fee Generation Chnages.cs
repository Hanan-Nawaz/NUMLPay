namespace NUMLPay_Api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FeeGenerationChnages : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UnpaidFees", "semester", c => c.Int(nullable: false));
            AddColumn("dbo.FeeInstallments", "paid_date", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.FeeInstallments", "paid_date");
            DropColumn("dbo.UnpaidFees", "semester");
        }
    }
}
