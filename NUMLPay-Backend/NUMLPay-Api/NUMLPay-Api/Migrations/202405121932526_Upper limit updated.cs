namespace NUMLPay_Api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Upperlimitupdated : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Departments", "name", c => c.String(nullable: false, maxLength: 200, unicode: false));
            AlterColumn("dbo.Faculties", "name", c => c.String(nullable: false, maxLength: 200, unicode: false));
            AlterColumn("dbo.Users", "name", c => c.String(nullable: false, maxLength: 200, unicode: false));
            AlterColumn("dbo.Degrees", "name", c => c.String(nullable: false, maxLength: 200, unicode: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Degrees", "name", c => c.String(nullable: false, maxLength: 50, unicode: false));
            AlterColumn("dbo.Users", "name", c => c.String(nullable: false, maxLength: 50, unicode: false));
            AlterColumn("dbo.Faculties", "name", c => c.String(nullable: false, maxLength: 30, unicode: false));
            AlterColumn("dbo.Departments", "name", c => c.String(nullable: false, maxLength: 50, unicode: false));
        }
    }
}
