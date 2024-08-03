namespace NUMLPay_Api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SubjectSummerFeeupdated : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Subjects", "added_by", c => c.String(maxLength: 30, unicode: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Subjects", "added_by");
        }
    }
}
