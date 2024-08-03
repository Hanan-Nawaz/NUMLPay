import sys
sys.path.append('e:\\University\\FYP\\Code\\NUMLPay\\ETL\\connection')

from connection import DatabaseConnection

def execute_sql_query(sql_query, conn):
    cursor = conn.cursor()
    cursor.execute(sql_query)
    rows = cursor.fetchall()
    cursor.close()
    return rows

def update_or_insert_data(rows, conn_dw):
    cursor_dw = conn_dw.cursor()
    cursor_dw.execute("Delete from HostelBusStudents")
    conn_dw.commit()
    for row in rows:
        dept_id = row[0]
        total_students = row[1]
        category = row[2]

        # Check if data for department and category exists in target table
        

        cursor_dw.execute("INSERT INTO HostelBusStudents (dept_id, total_students, category) VALUES (?, ?, ?)", (dept_id, total_students, category))

    conn_dw.commit()
    cursor_dw.close()

def execute_etl():

    # Source database connection parameters
    source_server = 'HANAN-NAWAZ\SQLEXPRESS'
    source_database = 'NUMLPay_db'

    # Data warehouse connection parameters
    dw_server = 'HANAN-NAWAZ\SQLEXPRESS'
    dw_database = 'NUMLPay_dw'

    # Construct SQL query
    sql_query = """
    SELECT D.faculty_id as dept_id, COUNT(U.numl_id) as total_students, 
    CASE
        When E.bus_fee = 1 THEN 'Using BUS'
        When E.hostel_fee = 1 THEN 'Living in Hostel'
    END as category
    FROM USERS AS U 
    LEFT JOIN EligibleFees AS E ON U.numl_id = E.std_numl_id  
    LEFT JOIN Departments AS D ON U.dept_id = D.id
	LEFT JOIN Faculties as F on D.faculty_id = f.id
    WHERE E.bus_fee = 1 OR E.hostel_fee = 1
    GROUP BY
        D.faculty_id,
        CASE
            When E.bus_fee = 1 THEN 'Using BUS'
            When E.hostel_fee = 1 THEN 'Living in Hostel'
        END;
    """

    # Establish connection to source database
    source_conn = DatabaseConnection(source_server, source_database)
    conn_source = source_conn.connect()

    # Execute SQL query and retrieve data
    rows = execute_sql_query(sql_query, conn_source)

    # Establish connection to data warehouse
    dw_conn = DatabaseConnection(dw_server, dw_database)
    conn_dw = dw_conn.connect()

    # Update or insert data into target table
    update_or_insert_data(rows, conn_dw)

    # Close connections
    source_conn.close(conn_source)
    dw_conn.close(conn_dw)



