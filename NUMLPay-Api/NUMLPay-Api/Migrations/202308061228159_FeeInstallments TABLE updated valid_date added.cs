namespace NUMLPay_Api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FeeInstallmentsTABLEupdatedvalid_dateadded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FeeInstallments", "valid_date", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.FeeInstallments", "valid_date");
        }
    }
}
