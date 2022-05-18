from typing import Any

from sqlalchemy import create_engine, inspect
from sqlalchemy.engine import URL


class BaseDBManager:
    def __init__(
        self,
        username: str,
        password: str,
        host: str,
        port: int,
        database: str,
        drivername: str,
    ) -> None:
        """Constructor method for DBManager classes.
        Creates the connection engine and the inpector for the database.

        Args:
            username (str): The username for the database.
            password (str): The password for the database.
            host (str): The host for the database.
            port (int): The port number for the database.
            database (str): The name of the database.
            drivername (str): The database driver e.g. "psycopg2", "pymysql", etc.
        """
        url = URL(
            drivername=drivername,
            username=username,
            password=password,
            host=host,
            port=port,
            database=database,
        )
        self.engine = create_engine(url=url)
        self.inspector = inspect(self.engine)

    def exectute_and_fetch(self, stmnt: Any) -> list:
        """Execute a statement against the database and fetch the result.

        Args:
            stmnt (Any): The statement object to be executed.

        Returns:
            list: The list of rows returned.
        """
        pass

    def execute(self, stmnt: Any) -> None:
        """Execute a statement against the database and don't fetch any results.

        Args:
            stmnt (Any): The statement object to be executed.
        """
        pass

    def list_tables(self) -> list:
        pass


class SyncDBManager(BaseDBManager):
    def exectute_and_fetch(self, stmnt: Any) -> list:
        with self.engine.begin() as conn:
            result = conn.execute(statement=stmnt)
            rows = result.all()
        return rows

    def execute(self, stmnt) -> None:
        with self.engine.begin() as conn:
            conn.execute(statement=stmnt)

    def list_tables(self) -> list:
        return self.inspector.get_table_names()
