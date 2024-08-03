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
    cursor_dw.execute("Delete from Degrees")
    conn_dw.commit()
    for row in rows:
        degree_id = row[0]
        degree_name = row[1]
        dept_id = row[2]

        # Check if data for the degree exists in the target table
        

        cursor_dw.execute("INSERT INTO Degrees (degree_id, degree_name, dept_id) VALUES (?, ?, ?)", (degree_id, degree_name, dept_id))

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
    SELECT 
        S.Id, 
        CASE
            WHEN S.academic_id = 1 AND S.shift = 1 THEN 'BS - ' + D.name + ' (Morning)'
            WHEN S.academic_id = 1 AND S.shift = 2 THEN 'BS - ' + D.name + ' (Evening)'
            WHEN S.academic_id = 2 AND S.shift = 1 THEN 'MS - ' + D.name  + ' (Morning)'
            WHEN S.academic_id = 2 AND S.shift = 2 THEN 'MS - ' + D.name  + ' (Evening)'
            WHEN S.academic_id = 3 AND S.shift = 1 THEN 'PHD - ' + D.name + ' (Morning)'
            WHEN S.academic_id = 3 AND S.shift = 2 THEN 'PHD - ' + D.name + ' (Evening)'
        END AS degree_name,
        D.dept_id
    FROM Degrees AS D 
    Right JOIN Shifts AS S ON D.id = S.degree_id
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
