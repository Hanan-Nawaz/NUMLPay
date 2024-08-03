import pyodbc

class DatabaseConnection:
    def __init__(self, server, database):
        self.server = server
        self.database = database

    def connect(self):
        # Establish connection to the database (using Windows authentication)
        conn_string = f'DRIVER={{SQL Server}};SERVER={self.server};DATABASE={self.database};Trusted_Connection=yes'
        conn = pyodbc.connect(conn_string)
        return conn

    def close(self, conn):
        # Close the database connection
        conn.close()
