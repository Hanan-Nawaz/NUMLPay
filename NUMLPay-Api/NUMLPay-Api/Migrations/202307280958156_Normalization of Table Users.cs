namespace NUMLPay_Api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NormalizationofTableUsers : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Users", "campus_id", "dbo.Campus");
            DropForeignKey("dbo.Users", "faculty_id", "dbo.Faculties");
            DropIndex("dbo.Users", new[] { "campus_id" });
            DropIndex("dbo.Users", new[] { "faculty_id" });
            DropColumn("dbo.Users", "campus_id");
            DropColumn("dbo.Users", "faculty_id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Users", "faculty_id", c => c.Int(nullable: false));
            AddColumn("dbo.Users", "campus_id", c => c.Int(nullable: false));
            CreateIndex("dbo.Users", "faculty_id");
            CreateIndex("dbo.Users", "campus_id");
            AddForeignKey("dbo.Users", "faculty_id", "dbo.Faculties", "id", cascadeDelete: true);
            AddForeignKey("dbo.Users", "campus_id", "dbo.Campus", "Id", cascadeDelete: true);
        }
    }
}
