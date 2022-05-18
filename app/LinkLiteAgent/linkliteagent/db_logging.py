from sqlalchemy import Column, Integer, String, DateTime, Text
from sqlalchemy.ext.declarative import declarative_base
import datetime


Base = declarative_base()


class Log(Base):
    __tablename__ = "LinkLiteLog"
    id = Column(Integer, primary_key=True, autoincrement=True)
    message = Column("Message", Text, nullable=True)
    message_template = Column("MessageTemplate", Text, nullable=True)
    level = Column("Level", String(128), nullable=True)
    timestamp = Column(
        "TimeStamp", DateTime, nullable=False, default=datetime.datetime.now
    )
    exception = Column("Exception", Text, nullable=True)
    properties = Column("Properties", Text, nullable=True)
