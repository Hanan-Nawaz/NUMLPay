namespace NUMLPay_Api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FeeStructureTablesUpdated : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.FeeStructures", "degree_id", "dbo.Degrees");
            DropIndex("dbo.FeeStructures", new[] { "degree_id" });
            AddColumn("dbo.FeeStructures", "shift_id", c => c.Int(nullable: false));
            AddColumn("dbo.SubFeeStructures", "admission_fee", c => c.Int(nullable: false));
            AddColumn("dbo.SubFeeStructures", "library_security", c => c.Int(nullable: false));
            AddColumn("dbo.SubFeeStructures", "registration_fee", c => c.Int(nullable: false));
            AlterColumn("dbo.FeeStructures", "session", c => c.Int(nullable: false));
            CreateIndex("dbo.FeeStructures", "shift_id");
            AddForeignKey("dbo.FeeStructures", "shift_id", "dbo.Shifts", "Id");
            DropColumn("dbo.FeeStructures", "degree_id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.FeeStructures", "degree_id", c => c.Int(nullable: false));
            DropForeignKey("dbo.FeeStructures", "shift_id", "dbo.Shifts");
            DropIndex("dbo.FeeStructures", new[] { "shift_id" });
            AlterColumn("dbo.FeeStructures", "session", c => c.String(nullable: false, maxLength: 30, unicode: false));
            DropColumn("dbo.SubFeeStructures", "registration_fee");
            DropColumn("dbo.SubFeeStructures", "library_security");
            DropColumn("dbo.SubFeeStructures", "admission_fee");
            DropColumn("dbo.FeeStructures", "shift_id");
            CreateIndex("dbo.FeeStructures", "degree_id");
            AddForeignKey("dbo.FeeStructures", "degree_id", "dbo.Degrees", "id", cascadeDelete: true);
        }
    }
}
