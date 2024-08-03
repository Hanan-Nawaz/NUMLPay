namespace NUMLPay_Api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NormalizationofTableUsersAcademicId : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Users", "academic_id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Users", "academic_id", c => c.Int(nullable: false));
        }
    }
}
