namespace NUMLPay_Api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChallansTableUpdatedis_activeAddeddatechanged : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Challans", "due_date", c => c.DateTime(nullable: false, storeType: "date"));
            AlterColumn("dbo.Challans", "valid_date", c => c.DateTime(nullable: false, storeType: "date"));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Challans", "valid_date", c => c.String(nullable: false));
            AlterColumn("dbo.Challans", "due_date", c => c.String(nullable: false));
        }
    }
}
