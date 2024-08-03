namespace NUMLPay_Api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserstabledegreeFktoShiftFK : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Users", "degree_id", "dbo.Degrees");
            AddForeignKey("dbo.Users", "degree_id", "dbo.Shifts", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Users", "degree_id", "dbo.Shifts");
            AddForeignKey("dbo.Users", "degree_id", "dbo.Degrees", "id", cascadeDelete: true);
        }
    }
}
