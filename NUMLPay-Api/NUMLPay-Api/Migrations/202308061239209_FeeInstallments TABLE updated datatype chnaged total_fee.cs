namespace NUMLPay_Api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FeeInstallmentsTABLEupdateddatatypechnagedtotal_fee : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.FeeInstallments", "total_fee", c => c.Single(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.FeeInstallments", "total_fee", c => c.Int(nullable: false));
        }
    }
}
