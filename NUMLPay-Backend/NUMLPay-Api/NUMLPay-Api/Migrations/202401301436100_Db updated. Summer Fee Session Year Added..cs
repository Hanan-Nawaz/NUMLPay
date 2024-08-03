namespace NUMLPay_Api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DbupdatedSummerFeeSessionYearAdded : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.SummerFees", "session_id", "dbo.Sessions");
            DropIndex("dbo.SummerFees", new[] { "session_id" });
            AddColumn("dbo.SummerFees", "session_year", c => c.Int(nullable: false));
            AddColumn("dbo.SummerFees", "added_by", c => c.String(maxLength: 30, unicode: false));
            DropColumn("dbo.SummerFees", "session_id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.SummerFees", "session_id", c => c.Int(nullable: false));
            DropColumn("dbo.SummerFees", "added_by");
            DropColumn("dbo.SummerFees", "session_year");
            CreateIndex("dbo.SummerFees", "session_id");
            AddForeignKey("dbo.SummerFees", "session_id", "dbo.Sessions", "id", cascadeDelete: true);
        }
    }
}
