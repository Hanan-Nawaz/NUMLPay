namespace NUMLPay_Api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NormalizationofTables : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Challans", "fine_after_10_days");
            DropColumn("dbo.Challans", "fine_after_30_days");
            DropColumn("dbo.Challans", "fine_after_60_days");
            DropColumn("dbo.Degrees", "academic_id");
            DropColumn("dbo.Degrees", "shift");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Degrees", "shift", c => c.Int(nullable: false));
            AddColumn("dbo.Degrees", "academic_id", c => c.Int(nullable: false));
            AddColumn("dbo.Challans", "fine_after_60_days", c => c.Int(nullable: false));
            AddColumn("dbo.Challans", "fine_after_30_days", c => c.Int(nullable: false));
            AddColumn("dbo.Challans", "fine_after_10_days", c => c.Int(nullable: false));
        }
    }
}
