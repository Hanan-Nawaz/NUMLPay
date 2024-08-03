namespace NUMLPay_Api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedBusRoutes : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BusRoutes",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        name = c.String(nullable: false, maxLength: 30, unicode: false),
                        campus_id = c.Int(nullable: false),
                        added_by = c.String(nullable: false, maxLength: 30, unicode: false),
                        is_active = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Campus", t => t.campus_id, cascadeDelete: true)
                .Index(t => t.campus_id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.BusRoutes", "campus_id", "dbo.Campus");
            DropIndex("dbo.BusRoutes", new[] { "campus_id" });
            DropTable("dbo.BusRoutes");
        }
    }
}
