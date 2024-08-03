import sys
sys.path.append('e:\\University\\FYP\\Code\\NUMLPay\\ETL\\connection')

from connection import DatabaseConnection

# Function to execute SQL query and retrieve data
def execute_sql_query(sql_query, conn):
    cursor = conn.cursor()
    cursor.execute(sql_query)
    rows = cursor.fetchall()
    cursor.close()
    return rows

# Function to update or insert data into target table
def update_or_insert_data(rows, conn_dw):
    cursor_dw = conn_dw.cursor()
    cursor_dw.execute("Delete from FacultyDepartments")
    conn_dw.commit()
    for row in rows:
        dept_id = row[0]
        dept_name = row[1]
        faculty_id = row[2]
        total_students = row[3]

        

        cursor_dw.execute("INSERT INTO FacultyDepartments (dept_id, dept_name, faculty_id, total_students) VALUES (?, ?, ?, ?)", (dept_id, dept_name, faculty_id, total_students))

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
       SELECT D.id, D.name, D.faculty_id, COUNT(U.numl_id) as total_students
    
    FROM USERS AS U 
    LEFT JOIN EligibleFees AS E ON U.numl_id = E.std_numl_id  
    LEFT JOIN Departments AS D ON U.dept_id = D.id
    GROUP BY
        D.id,
		D.name,
		D.faculty_id;
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

