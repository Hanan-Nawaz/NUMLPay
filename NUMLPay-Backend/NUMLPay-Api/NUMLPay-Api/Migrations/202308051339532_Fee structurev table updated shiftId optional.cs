namespace NUMLPay_Api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FeestructurevtableupdatedshiftIdoptional : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.FeeStructures", "shift_id", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.FeeStructures", "shift_id", c => c.Int(nullable: false));
        }
    }
}
