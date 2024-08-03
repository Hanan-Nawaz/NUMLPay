namespace NUMLPay_Api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UnpaidFeesTabledue_date : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.UnpaidFees", "paid_date", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.UnpaidFees", "paid_date", c => c.DateTime(nullable: false, storeType: "date"));
        }
    }
}
