namespace NUMLPay_Api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ShiftaddedtoDegree : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Degrees", "shift", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Degrees", "shift");
        }
    }
}
