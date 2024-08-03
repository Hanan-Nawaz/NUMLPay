namespace NUMLPay_Api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Ceasedimplemented : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Users", "is_ceased");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Users", "is_ceased", c => c.Int(nullable: false));
        }
    }
}
