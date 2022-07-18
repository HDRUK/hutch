import datetime
import argparse

from logging import getLogger, Handler, LogRecord
import dotenv

from sqlalchemy import create_engine, Column, Integer, String, DateTime, Text, insert
from sqlalchemy.engine import URL
from sqlalchemy.ext.declarative import declarative_base

from .db_manager import SyncDBManager

dotenv.load_dotenv()

Base = declarative_base()


class Logs(Base):
    __tablename__ = "Logs"  # this is required by sqlalchemy
    id = Column(Integer, primary_key=True, autoincrement=True)
    exception = Column(Text, nullable=True)
    level = Column(String(128), nullable=True)
    message = Column(Text, nullable=True)
    messagetemplate = Column(Text, nullable=True)
    properties = Column(Text, nullable=True)
    timestamp = Column(DateTime, nullable=False, default=datetime.datetime.now)


class SyncLogDBHandler(Handler):
    def __init__(self, db_manager: SyncDBManager, backup_logger: str, level=0) -> None:
        """Constructor for `SyncLogDBHandler`.

        Args:
            db_manager (SyncDBManager): The database manager.
            backup_logger (str): The name of the backup logger.
            level (int, optional): The numberic value for the log level. Defaults to 0.
        """
        super().__init__(level)
        self.db_manager = db_manager
        self.backup_logger = getLogger(backup_logger)

    def emit(self, record: LogRecord) -> None:
        """Record the logging record to the DB or to the backup logger

        Args:
            record (LogRecord): A `LogRecord` object.
        """
        # stack_info looks like `(type, value, traceback)` or `None`
        # https://docs.python.org/3/library/logging.html#logrecord-attributes
        if exc_info := record.stack_info:
            exception = exc_info[0]
        else:
            exception = None

        log_stmnt = insert(Logs).values(
            message=record.msg,
            level=record.levelname,
            exception=exception,
            messagetemplate=self.formatter._fmt,
        )

        try:
            self.db_manager.execute(log_stmnt)
        except Exception as e:
            self.backup_logger.error(str(e))


def create_log_table():
    parser = argparse.ArgumentParser()
    # Optional command line args
    parser.add_argument(
        "-n",
        "--port",
        dest="port",
        help="The port number for the database.",
        default=None,
        type=int,
    )
    parser.add_argument(
        "--host",
        dest="host",
        help="The host for the database.",
        default="localhost",
    )
    # Required command line args
    required_args = parser.add_argument_group("required arguments")
    required_args.add_argument(
        "--drivername",
        dest="drivername",
        help="""The driver for the database.
        Available drivers:
        - postgresql+psycopg2
        """,
        required=True,
    )
    required_args.add_argument(
        "-u",
        "--username",
        dest="username",
        help="The username for the database.",
        required=True,
    )
    required_args.add_argument(
        "-p",
        "--password",
        dest="password",
        help="The password for the database.",
        required=True,
    )
    required_args.add_argument(
        "-d",
        "--database",
        dest="database",
        help="The name for the database.",
        required=True,
    )
    # parse args
    args = parser.parse_args()

    url = URL(
        drivername=args.drivername,
        username=args.username,
        password=args.password,
        host=args.host,
        port=args.port,
        database=args.database,
    )
    engine = create_engine(url=url)
    Logs.metadata.create_all(engine, checkfirst=True)
