namespace NUMLPay_Api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DbCompletedandTablesAdded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AcademicLevels",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        name = c.String(nullable: false, maxLength: 50, unicode: false),
                        dept_id = c.Int(nullable: false),
                        added_by = c.String(nullable: false, maxLength: 30, unicode: false),
                        date = c.DateTime(nullable: false),
                        is_active = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Admins", t => t.added_by, cascadeDelete: true)
                .ForeignKey("dbo.Departments", t => t.dept_id, cascadeDelete: true)
                .Index(t => t.dept_id)
                .Index(t => t.added_by);
            
            CreateTable(
                "dbo.Admins",
                c => new
                    {
                        email_id = c.String(nullable: false, maxLength: 30, unicode: false),
                        password = c.String(nullable: false, maxLength: 15, unicode: false),
                        post = c.String(nullable: false, maxLength: 30, unicode: false),
                        campus_id = c.Int(),
                        faculty_id = c.Int(),
                        dept_id = c.Int(),
                        role = c.Int(nullable: false),
                        added_by = c.String(nullable: false, maxLength: 30, unicode: false),
                        is_active = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.email_id)
                .ForeignKey("dbo.Campus", t => t.campus_id)
                .ForeignKey("dbo.Departments", t => t.dept_id)
                .ForeignKey("dbo.Faculties", t => t.faculty_id)
                .Index(t => t.campus_id)
                .Index(t => t.faculty_id)
                .Index(t => t.dept_id);
            
            CreateTable(
                "dbo.Campus",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        name = c.String(nullable: false, maxLength: 50, unicode: false),
                        added_by = c.String(maxLength: 30, unicode: false),
                        date = c.DateTime(nullable: false, storeType: "date"),
                        is_active = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Departments",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        name = c.String(nullable: false, maxLength: 50, unicode: false),
                        faculty_id = c.Int(nullable: false),
                        added_by = c.String(nullable: false, maxLength: 30, unicode: false),
                        date = c.DateTime(nullable: false),
                        is_active = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Faculties", t => t.faculty_id, cascadeDelete: true)
                .Index(t => t.faculty_id);
            
            CreateTable(
                "dbo.Faculties",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        name = c.String(nullable: false, maxLength: 30, unicode: false),
                        campus_id = c.Int(nullable: false),
                        added_by = c.String(nullable: false, maxLength: 30, unicode: false),
                        date = c.DateTime(nullable: false),
                        is_active = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Campus", t => t.campus_id, cascadeDelete: true)
                .Index(t => t.campus_id);
            
            CreateTable(
                "dbo.AccountBooks",
                c => new
                    {
                        account_id = c.Int(nullable: false, identity: true),
                        std_numl_id = c.String(nullable: false, maxLength: 30, unicode: false),
                        challan_no = c.Int(nullable: false),
                        paid_date = c.DateTime(nullable: false, storeType: "date"),
                    })
                .PrimaryKey(t => t.account_id)
                .ForeignKey("dbo.UnpaidFees", t => t.challan_no, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.std_numl_id, cascadeDelete: true)
                .Index(t => t.std_numl_id)
                .Index(t => t.challan_no);
            
            CreateTable(
                "dbo.UnpaidFees",
                c => new
                    {
                        challan_no = c.Int(nullable: false, identity: true),
                        std_numl_id = c.String(maxLength: 30, unicode: false),
                        challan_id = c.Int(nullable: false),
                        fee_id = c.Int(nullable: false),
                        status = c.Int(nullable: false),
                        fee_installments = c.Int(nullable: false),
                        payment_method = c.Int(nullable: false),
                        image = c.String(maxLength: 8000, unicode: false),
                        paid_date = c.DateTime(nullable: false, storeType: "date"),
                        verified_by = c.String(maxLength: 30, unicode: false),
                    })
                .PrimaryKey(t => t.challan_no)
                .ForeignKey("dbo.Admins", t => t.verified_by)
                .ForeignKey("dbo.Challans", t => t.challan_id, cascadeDelete: true)
                .ForeignKey("dbo.FeeInstallments", t => t.fee_installments, cascadeDelete: true)
                .ForeignKey("dbo.FeeStructures", t => t.fee_id, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.std_numl_id)
                .Index(t => t.std_numl_id)
                .Index(t => t.challan_id)
                .Index(t => t.fee_id)
                .Index(t => t.fee_installments)
                .Index(t => t.verified_by);
            
            CreateTable(
                "dbo.Challans",
                c => new
                    {
                        challan_id = c.Int(nullable: false, identity: true),
                        current_session = c.String(nullable: false, maxLength: 50, unicode: false),
                        due_date = c.DateTime(nullable: false, storeType: "date"),
                        fine_after_10_days = c.Int(nullable: false),
                        fine_after_30_days = c.Int(nullable: false),
                        fine_after_60_days = c.Int(nullable: false),
                        fee_for = c.Int(nullable: false),
                        issue_date = c.DateTime(nullable: false, storeType: "date"),
                        added_by = c.String(maxLength: 30, unicode: false),
                    })
                .PrimaryKey(t => t.challan_id)
                .ForeignKey("dbo.Admins", t => t.added_by)
                .Index(t => t.added_by);
            
            CreateTable(
                "dbo.FeeInstallments",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        installment_id = c.Int(nullable: false),
                        installment_no = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.InstallmentManagements", t => t.installment_id, cascadeDelete: true)
                .Index(t => t.installment_id);
            
            CreateTable(
                "dbo.InstallmentManagements",
                c => new
                    {
                        installment_id = c.Int(nullable: false, identity: true),
                        mode = c.Int(nullable: false),
                        is_active = c.Int(nullable: false),
                        added_by = c.String(maxLength: 30, unicode: false),
                    })
                .PrimaryKey(t => t.installment_id)
                .ForeignKey("dbo.Admins", t => t.added_by)
                .Index(t => t.added_by);
            
            CreateTable(
                "dbo.FeeStructures",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        degree_id = c.Int(nullable: false),
                        session = c.String(nullable: false, maxLength: 30, unicode: false),
                        sub_structure_id = c.Int(nullable: false),
                        added_by = c.String(nullable: false, maxLength: 30, unicode: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Admins", t => t.added_by, cascadeDelete: true)
                .ForeignKey("dbo.Degrees", t => t.degree_id, cascadeDelete: true)
                .ForeignKey("dbo.SubFeeStructures", t => t.sub_structure_id, cascadeDelete: true)
                .Index(t => t.degree_id)
                .Index(t => t.sub_structure_id)
                .Index(t => t.added_by);
            
            CreateTable(
                "dbo.Degrees",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        name = c.String(nullable: false, maxLength: 50, unicode: false),
                        academic_id = c.Int(),
                        added_by = c.String( maxLength: 30, unicode: false),
                        date = c.DateTime(nullable: false),
                        is_active = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.AcademicLevels", t => t.academic_id)
                .ForeignKey("dbo.Admins", t => t.added_by)
                .Index(t => t.academic_id)
                .Index(t => t.added_by);
            
            CreateTable(
                "dbo.SubFeeStructures",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        audio_it = c.Int(nullable: false),
                        maintainence = c.Int(nullable: false),
                        computer_lab = c.Int(nullable: false),
                        tutition_fee = c.Int(nullable: false),
                        library = c.Int(nullable: false),
                        exam_fee = c.Int(nullable: false),
                        sports = c.Int(nullable: false),
                        medical = c.Int(nullable: false),
                        magazine = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        numl_id = c.String(nullable: false, maxLength: 30, unicode: false),
                        name = c.String(nullable: false, maxLength: 50, unicode: false),
                        father_name = c.String(nullable: false, maxLength: 50, unicode: false),
                        date_of_birth = c.DateTime(nullable: false, storeType: "date"),
                        email = c.String(nullable: false, maxLength: 50, unicode: false),
                        contact = c.String(nullable: false, maxLength: 50, unicode: false),
                        nic = c.String(nullable: false, maxLength: 15, unicode: false),
                        degree_id = c.Int(),
                        academic_id = c.Int(),
                        admission_session = c.String(nullable: false, maxLength: 20, unicode: false),
                        campus_id = c.Int(nullable: false),
                        faculty_id = c.Int(),
                        dept_id = c.Int(),
                        fee_plan = c.Int(nullable: false),
                        status_of_degree = c.Int(nullable: false),
                        is_active = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.numl_id)
                .ForeignKey("dbo.AcademicLevels", t => t.academic_id)
                .ForeignKey("dbo.Campus", t => t.campus_id, cascadeDelete: true)
                .ForeignKey("dbo.Degrees", t => t.degree_id)
                .ForeignKey("dbo.Departments", t => t.dept_id)
                .ForeignKey("dbo.Faculties", t => t.faculty_id)
                .ForeignKey("dbo.FeePlans", t => t.fee_plan, cascadeDelete: true)
                .Index(t => t.degree_id)
                .Index(t => t.academic_id)
                .Index(t => t.campus_id)
                .Index(t => t.faculty_id)
                .Index(t => t.dept_id)
                .Index(t => t.fee_plan);
            
            CreateTable(
                "dbo.FeePlans",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        name = c.String(nullable: false, maxLength: 50, unicode: false),
                        discount = c.Int(nullable: false),
                        added_by = c.String(maxLength: 30, unicode: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Admins", t => t.added_by)
                .Index(t => t.added_by);
            
            CreateTable(
                "dbo.EligibleFees",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        std_numl_id = c.String(nullable: false, maxLength: 30, unicode: false),
                        semester_fee = c.Int(nullable: false),
                        hostel_fee = c.Int(nullable: false),
                        bus_fee = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Users", t => t.std_numl_id, cascadeDelete: true)
                .Index(t => t.std_numl_id);
            
            CreateTable(
                "dbo.MiscellaneousFees",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        campus_id = c.Int(nullable: false),
                        desc = c.String(nullable: false, maxLength: 50, unicode: false),
                        amount = c.Int(nullable: false),
                        added_by = c.String(nullable: false, maxLength: 30, unicode: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Admins", t => t.added_by, cascadeDelete: true)
                .ForeignKey("dbo.Campus", t => t.campus_id, cascadeDelete: true)
                .Index(t => t.campus_id)
                .Index(t => t.added_by);
            
            CreateTable(
                "dbo.RequestLetters",
                c => new
                    {
                        request_id = c.Int(nullable: false, identity: true),
                        std_numl_id = c.String(nullable: false, maxLength: 30, unicode: false),
                        subject = c.String(nullable: false, maxLength: 8000, unicode: false),
                        body = c.String(nullable: false, maxLength: 8000, unicode: false),
                        supporting_material = c.String(maxLength: 8000, unicode: false),
                        date = c.DateTime(nullable: false, storeType: "date"),
                        status = c.Int(nullable: false),
                        decision_by = c.String(maxLength: 30, unicode: false),
                    })
                .PrimaryKey(t => t.request_id)
                .ForeignKey("dbo.Admins", t => t.decision_by)
                .ForeignKey("dbo.Users", t => t.std_numl_id, cascadeDelete: true)
                .Index(t => t.std_numl_id)
                .Index(t => t.decision_by);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RequestLetters", "std_numl_id", "dbo.Users");
            DropForeignKey("dbo.RequestLetters", "decision_by", "dbo.Admins");
            DropForeignKey("dbo.MiscellaneousFees", "campus_id", "dbo.Campus");
            DropForeignKey("dbo.MiscellaneousFees", "added_by", "dbo.Admins");
            DropForeignKey("dbo.EligibleFees", "std_numl_id", "dbo.Users");
            DropForeignKey("dbo.AccountBooks", "std_numl_id", "dbo.Users");
            DropForeignKey("dbo.AccountBooks", "challan_no", "dbo.UnpaidFees");
            DropForeignKey("dbo.UnpaidFees", "std_numl_id", "dbo.Users");
            DropForeignKey("dbo.Users", "fee_plan", "dbo.FeePlans");
            DropForeignKey("dbo.FeePlans", "added_by", "dbo.Admins");
            DropForeignKey("dbo.Users", "faculty_id", "dbo.Faculties");
            DropForeignKey("dbo.Users", "dept_id", "dbo.Departments");
            DropForeignKey("dbo.Users", "degree_id", "dbo.Degrees");
            DropForeignKey("dbo.Users", "campus_id", "dbo.Campus");
            DropForeignKey("dbo.Users", "academic_id", "dbo.AcademicLevels");
            DropForeignKey("dbo.UnpaidFees", "fee_id", "dbo.FeeStructures");
            DropForeignKey("dbo.FeeStructures", "sub_structure_id", "dbo.SubFeeStructures");
            DropForeignKey("dbo.FeeStructures", "degree_id", "dbo.Degrees");
            DropForeignKey("dbo.Degrees", "added_by", "dbo.Admins");
            DropForeignKey("dbo.Degrees", "academic_id", "dbo.AcademicLevels");
            DropForeignKey("dbo.FeeStructures", "added_by", "dbo.Admins");
            DropForeignKey("dbo.UnpaidFees", "fee_installments", "dbo.FeeInstallments");
            DropForeignKey("dbo.FeeInstallments", "installment_id", "dbo.InstallmentManagements");
            DropForeignKey("dbo.InstallmentManagements", "added_by", "dbo.Admins");
            DropForeignKey("dbo.UnpaidFees", "challan_id", "dbo.Challans");
            DropForeignKey("dbo.Challans", "added_by", "dbo.Admins");
            DropForeignKey("dbo.UnpaidFees", "verified_by", "dbo.Admins");
            DropForeignKey("dbo.AcademicLevels", "dept_id", "dbo.Departments");
            DropForeignKey("dbo.AcademicLevels", "added_by", "dbo.Admins");
            DropForeignKey("dbo.Admins", "faculty_id", "dbo.Faculties");
            DropForeignKey("dbo.Admins", "dept_id", "dbo.Departments");
            DropForeignKey("dbo.Departments", "faculty_id", "dbo.Faculties");
            DropForeignKey("dbo.Faculties", "campus_id", "dbo.Campus");
            DropForeignKey("dbo.Admins", "campus_id", "dbo.Campus");
            DropIndex("dbo.RequestLetters", new[] { "decision_by" });
            DropIndex("dbo.RequestLetters", new[] { "std_numl_id" });
            DropIndex("dbo.MiscellaneousFees", new[] { "added_by" });
            DropIndex("dbo.MiscellaneousFees", new[] { "campus_id" });
            DropIndex("dbo.EligibleFees", new[] { "std_numl_id" });
            DropIndex("dbo.FeePlans", new[] { "added_by" });
            DropIndex("dbo.Users", new[] { "fee_plan" });
            DropIndex("dbo.Users", new[] { "dept_id" });
            DropIndex("dbo.Users", new[] { "faculty_id" });
            DropIndex("dbo.Users", new[] { "campus_id" });
            DropIndex("dbo.Users", new[] { "academic_id" });
            DropIndex("dbo.Users", new[] { "degree_id" });
            DropIndex("dbo.Degrees", new[] { "added_by" });
            DropIndex("dbo.Degrees", new[] { "academic_id" });
            DropIndex("dbo.FeeStructures", new[] { "added_by" });
            DropIndex("dbo.FeeStructures", new[] { "sub_structure_id" });
            DropIndex("dbo.FeeStructures", new[] { "degree_id" });
            DropIndex("dbo.InstallmentManagements", new[] { "added_by" });
            DropIndex("dbo.FeeInstallments", new[] { "installment_id" });
            DropIndex("dbo.Challans", new[] { "added_by" });
            DropIndex("dbo.UnpaidFees", new[] { "verified_by" });
            DropIndex("dbo.UnpaidFees", new[] { "fee_installments" });
            DropIndex("dbo.UnpaidFees", new[] { "fee_id" });
            DropIndex("dbo.UnpaidFees", new[] { "challan_id" });
            DropIndex("dbo.UnpaidFees", new[] { "std_numl_id" });
            DropIndex("dbo.AccountBooks", new[] { "challan_no" });
            DropIndex("dbo.AccountBooks", new[] { "std_numl_id" });
            DropIndex("dbo.Faculties", new[] { "campus_id" });
            DropIndex("dbo.Departments", new[] { "faculty_id" });
            DropIndex("dbo.Admins", new[] { "dept_id" });
            DropIndex("dbo.Admins", new[] { "faculty_id" });
            DropIndex("dbo.Admins", new[] { "campus_id" });
            DropIndex("dbo.AcademicLevels", new[] { "added_by" });
            DropIndex("dbo.AcademicLevels", new[] { "dept_id" });
            DropTable("dbo.RequestLetters");
            DropTable("dbo.MiscellaneousFees");
            DropTable("dbo.EligibleFees");
            DropTable("dbo.FeePlans");
            DropTable("dbo.Users");
            DropTable("dbo.SubFeeStructures");
            DropTable("dbo.Degrees");
            DropTable("dbo.FeeStructures");
            DropTable("dbo.InstallmentManagements");
            DropTable("dbo.FeeInstallments");
            DropTable("dbo.Challans");
            DropTable("dbo.UnpaidFees");
            DropTable("dbo.AccountBooks");
            DropTable("dbo.Faculties");
            DropTable("dbo.Departments");
            DropTable("dbo.Campus");
            DropTable("dbo.Admins");
            DropTable("dbo.AcademicLevels");
        }
    }
}
