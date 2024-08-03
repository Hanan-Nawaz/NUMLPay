from account_etl import main as main_accountant_etl
from campus_etl import main as main_campus_etl
from dept_etl import main as main_dept_etl
from super_etl import main as main_super_etl

def main():
    main_accountant_etl()
    main_campus_etl()
    main_dept_etl()
    main_super_etl()

if __name__ == "__main__":
    main()
