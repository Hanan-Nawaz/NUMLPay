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
    for row in rows:
        faculty_name = row[1]
        campus_id = row[2]
        faculty_id = row[0]

        # Check if data for faculty exists in target table
        cursor_dw.execute("Delete from Faculties")

            # Data for faculty does not exist, insert new record
        cursor_dw.execute("INSERT INTO Faculties (faculty_id, faculty_name, campus_id) VALUES (?, ?, ?)", (faculty_id, faculty_name, campus_id))

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
    SELECT F.id, F.name, C.id
    FROM Faculties AS F
    LEFT JOIN Campus AS C ON F.campus_id = C.id
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


execute_etl()