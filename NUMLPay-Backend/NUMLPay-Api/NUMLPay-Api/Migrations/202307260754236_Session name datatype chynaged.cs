namespace NUMLPay_Api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Sessionnamedatatypechynaged : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Sessions", "name", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Sessions", "name", c => c.String(nullable: false, maxLength: 30, unicode: false));
        }
    }
}
