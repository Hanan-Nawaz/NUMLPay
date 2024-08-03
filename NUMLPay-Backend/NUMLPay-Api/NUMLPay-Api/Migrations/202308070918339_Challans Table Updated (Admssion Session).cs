namespace NUMLPay_Api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChallansTableUpdatedAdmssionSession : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Challans", "admissison_session", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Challans", "admissison_session");
        }
    }
}
