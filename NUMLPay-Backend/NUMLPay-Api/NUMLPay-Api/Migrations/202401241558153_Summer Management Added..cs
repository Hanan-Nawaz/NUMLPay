namespace NUMLPay_Api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SummerManagementAdded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Subjects",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        name = c.String(nullable: false, maxLength: 50, unicode: false),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.SummerEnrollments",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        subject_id = c.Int(nullable: false),
                        std_numl_id = c.String(nullable: false, maxLength: 30, unicode: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Subjects", t => t.subject_id, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.std_numl_id, cascadeDelete: true)
                .Index(t => t.subject_id)
                .Index(t => t.std_numl_id);
            
            CreateTable(
                "dbo.SummerFees",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        fee = c.Int(nullable: false),
                        subject_id = c.Int(nullable: false),
                        session_id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Sessions", t => t.session_id, cascadeDelete: true)
                .ForeignKey("dbo.Subjects", t => t.subject_id, cascadeDelete: true)
                .Index(t => t.subject_id)
                .Index(t => t.session_id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SummerFees", "subject_id", "dbo.Subjects");
            DropForeignKey("dbo.SummerFees", "session_id", "dbo.Sessions");
            DropForeignKey("dbo.SummerEnrollments", "std_numl_id", "dbo.Users");
            DropForeignKey("dbo.SummerEnrollments", "subject_id", "dbo.Subjects");
            DropIndex("dbo.SummerFees", new[] { "session_id" });
            DropIndex("dbo.SummerFees", new[] { "subject_id" });
            DropIndex("dbo.SummerEnrollments", new[] { "std_numl_id" });
            DropIndex("dbo.SummerEnrollments", new[] { "subject_id" });
            DropTable("dbo.SummerFees");
            DropTable("dbo.SummerEnrollments");
            DropTable("dbo.Subjects");
        }
    }
}
