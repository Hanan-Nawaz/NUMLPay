namespace NUMLPay_Api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FinesTableUpdatedSessionandfien_foradded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Fines", "fine_for", c => c.Int(nullable: false));
            AddColumn("dbo.Fines", "session", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Fines", "session");
            DropColumn("dbo.Fines", "fine_for");
        }
    }
}
