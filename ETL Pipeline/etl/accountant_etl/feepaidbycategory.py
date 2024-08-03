import pyodbc

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
    cursor_dw.execute("Delete from FeePaidByCategory")
    conn_dw.commit()
    for row in rows:
        fee_paid = row[0]
        campus_name = row[1]
        category = row[2]

       
        # Data for campus and category does not exist, insert new record
        cursor_dw.execute("INSERT INTO FeePaidByCategory (campus_name, category, fee_paid) VALUES (?, ?, ?)", (campus_name, category, fee_paid))

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
        C.name AS campus_name,
        CASE 
            WHEN FS.fee_for = 1 THEN 'Tuition Fee'
            WHEN FS.fee_for = 2 THEN 'Bus Fee'
            WHEN FS.fee_for = 3 THEN 'Hostel fee'
            WHEN FS.fee_for = 4 THEN 'Mess Fee'
            ELSE ''
        END AS category
    FROM FeeInstallments AS FI
    LEFT JOIN UnpaidFees AS UPF ON FI.challan_id = UPF.challan_no
    LEFT JOIN FeeStructures AS FS ON UPF.fee_id = FS.Id 
    LEFT JOIN Users AS US ON UPF.std_numl_id = US.numl_id
    LEFT JOIN Departments AS D ON US.dept_id = D.id
    LEFT JOIN Faculties AS F ON D.faculty_id = F.id
    LEFT JOIN Campus AS C ON F.campus_id = C.Id
    WHERE 
        FI.status = 1
        AND
        UPF.fee_type IN (1, 3)
    GROUP BY
        C.name,
        FS.fee_for;
    """

    # Establish connection to source database
    source_conn = pyodbc.connect('DRIVER={SQL Server};SERVER='+source_server+';DATABASE='+source_database+';Trusted_Connection=yes')

    # Execute SQL query and retrieve data
    rows = execute_sql_query(sql_query, source_conn)

    # Establish connection to data warehouse
    dw_conn = pyodbc.connect('DRIVER={SQL Server};SERVER='+dw_server+';DATABASE='+dw_database+';Trusted_Connection=yes')

    # Update or insert data into target table
    update_or_insert_data(rows, dw_conn)

    # Close connections
    source_conn.close()
    dw_conn.close()
