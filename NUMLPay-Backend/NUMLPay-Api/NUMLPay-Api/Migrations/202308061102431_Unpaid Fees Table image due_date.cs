namespace NUMLPay_Api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UnpaidFeesTableimagedue_date : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.UnpaidFees", "image", c => c.String(maxLength: 8000, unicode: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.UnpaidFees", "image", c => c.String(nullable: false, maxLength: 8000, unicode: false));
        }
    }
}
