import sys
sys.path.append('e:\\University\\FYP\\Code\\NUMLPay\\ETL\\etl')

from etl.supers_etl.studentsincampus import execute_etl as execute_by_std_in_campus
from etl.supers_etl.campus import execute_etl as execute_by_campus

def main():
    execute_by_std_in_campus()
    execute_by_campus()

if __name__ == "__main__":
    main()
