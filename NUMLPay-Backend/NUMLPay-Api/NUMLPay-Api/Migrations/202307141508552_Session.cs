namespace NUMLPay_Api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Session : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Sessions",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        name = c.String(nullable: false, maxLength: 30, unicode: false),
                        year = c.Int(nullable: false),
                        added_by = c.String(maxLength: 30, unicode: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Admins", t => t.added_by)
                .Index(t => t.added_by);
            
            AddColumn("dbo.Users", "verified_by", c => c.String(maxLength: 30, unicode: false));
            CreateIndex("dbo.Users", "verified_by");
            AddForeignKey("dbo.Users", "verified_by", "dbo.Admins", "email_id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Sessions", "added_by", "dbo.Admins");
            DropForeignKey("dbo.Users", "verified_by", "dbo.Admins");
            DropIndex("dbo.Sessions", new[] { "added_by" });
            DropIndex("dbo.Users", new[] { "verified_by" });
            DropColumn("dbo.Users", "verified_by");
            DropTable("dbo.Sessions");
        }
    }
}
