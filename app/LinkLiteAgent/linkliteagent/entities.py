from sqlalchemy import Column, Integer, String, DateTime, Text
from sqlalchemy.ext.declarative import declarative_base

Base = declarative_base()

class Person(Base):
    __tablename__ = "person"
    person_id = Column(Integer, primary_key=True)
    gender_concept_id = Column(Integer, nullable=False)
    year_of_birth = Column(Integer, nullable=False)
    month_of_birth = Column(Integer, nullable=True)
    day_of_birth = Column(Integer, nullable=True)
    birth_datetime = Column(DateTime, nullable=True)
    race_concept_id = Column(Integer, nullable=False)
    ethnicity_concept_id = Column(Integer, nullable=False)
    location_id = Column(Integer, nullable=True)
    provider_id = Column(Integer, nullable=True)
    care_site_id = Column(Integer, nullable=True)
    person_source_value = Column(String(50), nullable=True)
    gender_source_value = Column(String(50), nullable=True)
    gender_source_concept_id = Column(Integer, nullable=True)
    race_source_value = Column(String(50), nullable=True)
    race_source_concept_id = Column(Integer, nullable=True)
    ethnicity_source_value = Column(String(50), nullable=True)
    ethnicity_source_concept_id = Column(Integer, nullable=True)
