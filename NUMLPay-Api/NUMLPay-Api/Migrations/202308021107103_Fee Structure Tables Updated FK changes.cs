namespace NUMLPay_Api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FeeStructureTablesUpdatedFKchanges : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.FeeStructures", "sub_structure_id", "dbo.SubFeeStructures");
            DropIndex("dbo.FeeStructures", new[] { "sub_structure_id" });
            AddColumn("dbo.SubFeeStructures", "fee_structure_id", c => c.Int(nullable: false));
            CreateIndex("dbo.SubFeeStructures", "fee_structure_id");
            AddForeignKey("dbo.SubFeeStructures", "fee_structure_id", "dbo.FeeStructures", "Id", cascadeDelete: true);
            DropColumn("dbo.FeeStructures", "sub_structure_id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.FeeStructures", "sub_structure_id", c => c.Int(nullable: false));
            DropForeignKey("dbo.SubFeeStructures", "fee_structure_id", "dbo.FeeStructures");
            DropIndex("dbo.SubFeeStructures", new[] { "fee_structure_id" });
            DropColumn("dbo.SubFeeStructures", "fee_structure_id");
            CreateIndex("dbo.FeeStructures", "sub_structure_id");
            AddForeignKey("dbo.FeeStructures", "sub_structure_id", "dbo.SubFeeStructures", "Id", cascadeDelete: true);
        }
    }
}
