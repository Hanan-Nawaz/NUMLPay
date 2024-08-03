import sys
sys.path.append('e:\\University\\FYP\\Code\\NUMLPay\\ETL\\etl')

from etl.campuses_etl.department import execute_etl as execute_by_dept
from etl.campuses_etl.faculty import execute_etl as execute_by_faculty
from etl.campuses_etl.hostelbusstudents import execute_etl as execute_by_hostel_bus_stds

def main():
    execute_by_dept()
    execute_by_faculty()
    execute_by_hostel_bus_stds()

if __name__ == "__main__":
    main()
