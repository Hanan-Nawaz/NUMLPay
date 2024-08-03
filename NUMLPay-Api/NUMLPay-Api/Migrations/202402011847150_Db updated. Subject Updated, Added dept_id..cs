namespace NUMLPay_Api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DbupdatedSubjectUpdatedAddeddept_id : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Subjects", "dept_id", c => c.Int());
            CreateIndex("dbo.Subjects", "dept_id");
            AddForeignKey("dbo.Subjects", "dept_id", "dbo.Departments", "id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Subjects", "dept_id", "dbo.Departments");
            DropIndex("dbo.Subjects", new[] { "dept_id" });
            DropColumn("dbo.Subjects", "dept_id");
        }
    }
}
