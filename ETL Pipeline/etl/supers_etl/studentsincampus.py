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
    cursor_dw.execute("Delete from StudentsInCampus")
    conn_dw.commit()
    for row in rows:
        campus_id = row[0]
        total_students = row[1]

        # Check if data for the campus ID exists in the target table
        

        cursor_dw.execute("INSERT INTO StudentsInCampus (campus_id, total_students) VALUES (?, ?)", (campus_id, total_students))

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
    SELECT C.Id, COUNT(U.numl_id) AS total_students 
    FROM USERS AS U 
    LEFT JOIN Departments AS D ON U.dept_id = D.id 
    LEFT JOIN Faculties AS F ON D.faculty_id = F.id 
    LEFT JOIN Campus AS C ON F.campus_id = C.Id 
    GROUP BY C.Id
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


