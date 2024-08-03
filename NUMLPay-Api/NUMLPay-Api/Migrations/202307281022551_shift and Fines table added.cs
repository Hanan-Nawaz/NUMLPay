namespace NUMLPay_Api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class shiftandFinestableadded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Fines",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        fine_after_10_days = c.Int(nullable: false),
                        fine_after_30_days = c.Int(nullable: false),
                        fine_after_60_days = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.Shifts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        academic_id = c.Int(nullable: false),
                        shift = c.Int(nullable: false),
                        degree_id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Degrees", t => t.degree_id)
                .Index(t => t.degree_id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Shifts", "degree_id", "dbo.Degrees");
            DropIndex("dbo.Shifts", new[] { "degree_id" });
            DropTable("dbo.Shifts");
            DropTable("dbo.Fines");
        }
    }
}
