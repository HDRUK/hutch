from sqlalchemy import create_engine, inspect
from sqlalchemy.engine import URL


class BaseDBManager:
    drivername = None

    def __init__(
        self, username: str, password: str, host: str, port: int, database: str
    ) -> None:
        url = URL(
            drivername=self.drivername,
            username=username,
            password=password,
            host=host,
            port=port,
            database=database,
        )
        self.engine = create_engine(url=url)
        self.inspector = inspect(self.engine)

    def exectute(self, stmnt) -> None:
        pass

    def list_tables(self) -> list:
        pass
