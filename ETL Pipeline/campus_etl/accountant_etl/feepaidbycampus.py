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
        fee_paid = row[0]
        campus_name = row[1]

        # Check if data for campus exists in target table
        cursor_dw.execute("SELECT COUNT(*) FROM FeePaidByCampus WHERE campus_name = ?", (campus_name,))
        row_count = cursor_dw.fetchone()[0]

        if row_count > 0:
            # Data for campus exists, update fee_paid
            cursor_dw.execute("UPDATE FeePaidByCampus SET fee_paid = ? WHERE campus_name = ?", (fee_paid, campus_name))
        else:
            # Data for campus does not exist, insert new record
            cursor_dw.execute("INSERT INTO FeePaidByCampus (campus_name, fee_paid) VALUES (?, ?)", (campus_name, fee_paid))

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
        SUM(FI.total_fee) AS fee_paid,
        C.name AS campus_name
    FROM FeeInstallments AS FI
    LEFT JOIN UnpaidFees AS UPF ON FI.challan_id = UPF.challan_no
    LEFT JOIN Users AS US ON UPF.std_numl_id = US.numl_id
    LEFT JOIN Departments AS D ON US.dept_id = D.id
    LEFT JOIN Faculties AS F ON D.faculty_id = F.id
    LEFT JOIN Campus AS C ON F.campus_id = C.Id
    WHERE FI.status = 1
    GROUP BY
        C.name;
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
