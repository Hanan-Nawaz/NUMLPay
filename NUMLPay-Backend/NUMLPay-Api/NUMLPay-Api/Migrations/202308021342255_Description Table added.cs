namespace NUMLPay_Api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DescriptionTableadded : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.MiscellaneousFees", "campus_id", "dbo.Campus");
            DropIndex("dbo.MiscellaneousFees", new[] { "campus_id" });
            CreateTable(
                "dbo.Descriptions",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        name = c.String(nullable: false, maxLength: 50, unicode: false),
                        is_active = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id);
            
            AddColumn("dbo.MiscellaneousFees", "desc_id", c => c.Int(nullable: false));
            AddColumn("dbo.MiscellaneousFees", "session", c => c.Int(nullable: false));
            CreateIndex("dbo.MiscellaneousFees", "desc_id");
            AddForeignKey("dbo.MiscellaneousFees", "desc_id", "dbo.Descriptions", "id", cascadeDelete: true);
            DropColumn("dbo.MiscellaneousFees", "campus_id");
            DropColumn("dbo.MiscellaneousFees", "desc");
        }
        
        public override void Down()
        {
            AddColumn("dbo.MiscellaneousFees", "desc", c => c.String(nullable: false, maxLength: 50, unicode: false));
            AddColumn("dbo.MiscellaneousFees", "campus_id", c => c.Int(nullable: false));
            DropForeignKey("dbo.MiscellaneousFees", "desc_id", "dbo.Descriptions");
            DropIndex("dbo.MiscellaneousFees", new[] { "desc_id" });
            DropColumn("dbo.MiscellaneousFees", "session");
            DropColumn("dbo.MiscellaneousFees", "desc_id");
            DropTable("dbo.Descriptions");
            CreateIndex("dbo.MiscellaneousFees", "campus_id");
            AddForeignKey("dbo.MiscellaneousFees", "campus_id", "dbo.Campus", "Id", cascadeDelete: true);
        }
    }
}
