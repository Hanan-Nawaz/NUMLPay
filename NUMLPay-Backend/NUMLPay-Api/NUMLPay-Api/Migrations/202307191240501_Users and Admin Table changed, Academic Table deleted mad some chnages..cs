namespace NUMLPay_Api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UsersandAdminTablechangedAcademicTabledeletedmadsomechnages : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.Degrees", "dept_id");
            AddForeignKey("dbo.Degrees", "dept_id", "dbo.Departments", "id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Degrees", "dept_id", "dbo.Departments");
            DropIndex("dbo.Degrees", new[] { "dept_id" });
        }
    }
}
