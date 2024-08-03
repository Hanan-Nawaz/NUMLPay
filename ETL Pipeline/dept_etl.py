import sys
sys.path.append('e:\\University\\FYP\\Code\\NUMLPay\\ETL\\etl')

from etl.depts_etl.degrees import execute_etl as execute_by_degree
from etl.depts_etl.students import execute_etl as execute_by_students
from etl.depts_etl.ceasedstudents import execute_etl as execute_by_cesaed_students

def main():
    execute_by_degree()
    execute_by_students()
    execute_by_cesaed_students()

if __name__ == "__main__":
    main()
