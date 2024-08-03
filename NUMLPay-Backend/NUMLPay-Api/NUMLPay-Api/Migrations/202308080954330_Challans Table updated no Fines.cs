namespace NUMLPay_Api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChallansTableupdatednoFines : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Challans", "fine_id", "dbo.Fines");
            DropIndex("dbo.Challans", new[] { "fine_id" });
            DropColumn("dbo.Challans", "fine_id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Challans", "fine_id", c => c.Int(nullable: false));
            CreateIndex("dbo.Challans", "fine_id");
            AddForeignKey("dbo.Challans", "fine_id", "dbo.Fines", "id", cascadeDelete: true);
        }
    }
}
