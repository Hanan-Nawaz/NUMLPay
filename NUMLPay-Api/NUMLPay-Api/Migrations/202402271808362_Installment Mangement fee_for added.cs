namespace NUMLPay_Api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InstallmentMangementfee_foradded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.InstallmentManagements", "fee_for", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.InstallmentManagements", "fee_for");
        }
    }
}
