namespace NUMLPay_Api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DbupdatedSummerEnrollmentis_activedeleted : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.SummerEnrollments", "subject_id", "dbo.Subjects");
            DropIndex("dbo.SummerEnrollments", new[] { "subject_id" });
            AddColumn("dbo.SummerEnrollments", "summer_fee_id", c => c.Int(nullable: false));
            AddColumn("dbo.SummerEnrollments", "added_by", c => c.String(maxLength: 30, unicode: false));
            CreateIndex("dbo.SummerEnrollments", "summer_fee_id");
            AddForeignKey("dbo.SummerEnrollments", "summer_fee_id", "dbo.SummerFees", "id", cascadeDelete: true);
            DropColumn("dbo.SummerEnrollments", "subject_id");
            DropColumn("dbo.SummerEnrollments", "is_active");
        }
        
        public override void Down()
        {
            AddColumn("dbo.SummerEnrollments", "is_active", c => c.Int(nullable: false));
            AddColumn("dbo.SummerEnrollments", "subject_id", c => c.Int(nullable: false));
            DropForeignKey("dbo.SummerEnrollments", "summer_fee_id", "dbo.SummerFees");
            DropIndex("dbo.SummerEnrollments", new[] { "summer_fee_id" });
            DropColumn("dbo.SummerEnrollments", "added_by");
            DropColumn("dbo.SummerEnrollments", "summer_fee_id");
            CreateIndex("dbo.SummerEnrollments", "subject_id");
            AddForeignKey("dbo.SummerEnrollments", "subject_id", "dbo.Subjects", "id", cascadeDelete: true);
        }
    }
}
