namespace NUMLPay_Api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FeeInstallmentstableupdated : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FeeInstallments", "total_fee", c => c.Int(nullable: false));
            AddColumn("dbo.FeeInstallments", "due_date", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.FeeInstallments", "due_date");
            DropColumn("dbo.FeeInstallments", "total_fee");
        }
    }
}
