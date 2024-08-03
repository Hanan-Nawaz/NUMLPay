namespace NUMLPay_Api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FeeInstallmentsTableUpdatedFine : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FeeInstallments", "fine", c => c.Single(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.FeeInstallments", "fine");
        }
    }
}
