import sys
sys.path.append('e:\\University\\FYP\\Code\\NUMLPay\\ETL\\etl')

from etl.accountant_etl.feepaidbycampus import execute_etl as execute_fee_paid_by_campus_etl
from etl.accountant_etl.feepaidbycategory import execute_etl as execute_fee_paid_by_category_etl

def main():
    # Execute ETL for FeePaidByCampus
    execute_fee_paid_by_campus_etl()

    # Execute ETL for FeePaidByCategory
    execute_fee_paid_by_category_etl()

if __name__ == "__main__":
    main()
