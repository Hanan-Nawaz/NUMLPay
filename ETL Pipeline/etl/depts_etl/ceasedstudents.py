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
    cursor_dw.execute("Delete from CeasedStudents")
    conn_dw.commit()
    for row in rows:
        degree_id = row[0]
        total_students = row[1]

        # Check if data for the degree ID exists in the target table
        


        cursor_dw.execute("INSERT INTO CeasedStudents (degree_id, total_students) VALUES (?, ?)", (degree_id, total_students))
      
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
WITH FirstQuery AS (
    SELECT 
        U.degree_id,
        COUNT(U.numl_id) AS total_students
    FROM 
        Users AS U
    LEFT JOIN
        Shifts AS S ON U.degree_id = S.Id
    LEFT JOIN
        UnpaidFees AS UF ON U.numl_id = UF.std_numl_id AND UF.fee_type = 1
    LEFT JOIN
        FeeInstallments AS FI ON UF.challan_no = FI.challan_id AND (FI.status != 1)
    GROUP BY
        U.degree_id
),
SecondQuery AS (
    SELECT 
        U.degree_id,
        COUNT(U.numl_id) AS total_students
    FROM 
        Users AS U
    LEFT JOIN
        Shifts AS S ON U.degree_id = S.Id
    JOIN
        UnpaidFees AS UF ON U.numl_id = UF.std_numl_id AND UF.fee_type = 1
    GROUP BY
        U.degree_id
)
SELECT 
    FQ.degree_id,
    FQ.total_students - COALESCE(SQ.total_students, 0) AS total_students
FROM 
    FirstQuery FQ
LEFT JOIN 
    SecondQuery SQ ON FQ.degree_id = SQ.degree_id
;

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
