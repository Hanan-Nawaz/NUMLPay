namespace NUMLPay_Api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChallansTableUpdatedis_activeAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Challans", "is_active", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Challans", "is_active");
        }
    }
}
