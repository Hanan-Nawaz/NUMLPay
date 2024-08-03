namespace NUMLPay_Api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FeeSecurityTableAdded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FeeSecurities",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        fee_structure_id = c.Int(nullable: false),
                        security = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.FeeStructures", t => t.fee_structure_id, cascadeDelete: true)
                .Index(t => t.fee_structure_id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.FeeSecurities", "fee_structure_id", "dbo.FeeStructures");
            DropIndex("dbo.FeeSecurities", new[] { "fee_structure_id" });
            DropTable("dbo.FeeSecurities");
        }
    }
}
