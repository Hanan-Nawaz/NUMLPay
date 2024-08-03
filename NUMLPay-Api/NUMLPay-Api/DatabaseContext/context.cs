using NUMLPay_Api.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace NUMLPay_Api.DatabaseContext
{
    public class context : DbContext
    {
        public context() : base("dbcon") { }

        public DbSet<AccountBook> accountBooks { get; set; }
        public DbSet<Admin> admin { get; set; }
        public DbSet<Campus> campus { get; set; }
        public DbSet<Challans> challans { get; set; }
        public DbSet<Degree> degree { get; set; }
        public DbSet<Department> departments { get; set; }
        public DbSet<EligibleFees> eligibleFees { get; set; }
        public DbSet<Faculty> faculties { get; set; }
        public DbSet<FeeInstallments> feeInstallments { get; set; }
        public DbSet<FeePlan> feePlans { get; set; }
        public DbSet<FeeStructure> feeStructures { get; set; }
        public DbSet<InstallmentManagement> installmentManagements { get; set; }
        public DbSet<MiscellaneousFees> miscellaneousFees { get; set; }
        public DbSet<RequestLetter> requestLetters { get; set; }
        public DbSet<SubFeeStructure> subFeeStructures { get; set; }
        public DbSet<UnpaidFees> unpaidFees { get; set; }
        public DbSet<Users> users { get; set; }
        public DbSet<Session> sessions { get; set; }
        public DbSet<Shift> shift { get; set; }
        public DbSet<Fines> fine { get; set; }
        public DbSet<Description> descriptions { get; set; }
        public DbSet<ChallanVerification> challanVerification { get; set; }
        public DbSet<FeeSecurity> feeSecurity { get; set; }
        public DbSet<Subjects> subjects { get; set; }
        public DbSet<SummerFee> summerFees { get; set; }
        public DbSet<SummerEnrollment> summerEnrollments { get; set; }
        public DbSet<BusRoute> busRoutes { get; set; }

    }
}