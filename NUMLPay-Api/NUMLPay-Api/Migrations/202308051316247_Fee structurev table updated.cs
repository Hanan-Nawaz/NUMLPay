namespace NUMLPay_Api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Feestructurevtableupdated : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FeeStructures", "fee_for", c => c.Int(nullable: false));
            AddColumn("dbo.FeeStructures", "total_fee", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.FeeStructures", "total_fee");
            DropColumn("dbo.FeeStructures", "fee_for");
        }
    }
}
