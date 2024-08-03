namespace NUMLPay_Api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FeestructurevtableupdatedshiftId : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.FeeStructures", "shift_id", "dbo.Shifts");
            DropIndex("dbo.FeeStructures", new[] { "shift_id" });
        }
        
        public override void Down()
        {
            CreateIndex("dbo.FeeStructures", "shift_id");
            AddForeignKey("dbo.FeeStructures", "shift_id", "dbo.Shifts", "Id", cascadeDelete: true);
        }
    }
}
