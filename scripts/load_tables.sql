COPY concept FROM './CONCEPT.csv' DELIMITER E'\t' QUOTE E'\b' CSV HEADER;

COPY concept_ancestor FROM './CONCEPT_ANCESTOR.csv' DELIMITER E'\t' QUOTE E'\b' CSV HEADER;

COPY concept_class FROM './CONCEPT_CLASS.csv' DELIMITER E'\t' QUOTE E'\b' CSV HEADER;

COPY concept_relationship FROM './CONCEPT_RELATIONSHIP.csv' DELIMITER E'\t' QUOTE E'\b' CSV HEADER;

COPY concept_synonym FROM './CONCEPT_SYNONYM.csv' DELIMITER E'\t' QUOTE E'\b' CSV HEADER;

COPY domain FROM './DOMAIN.csv' DELIMITER E'\t' QUOTE E'\b' CSV HEADER;

COPY drug_strength FROM './DRUG_STRENGTH.csv' DELIMITER E'\t' QUOTE E'\b' CSV HEADER;

COPY relationship FROM './RELATIONSHIP.csv' DELIMITER E'\t' QUOTE E'\b' CSV HEADER;

COPY vocabulary FROM './VOCABULARY.csv' DELIMITER E'\t' QUOTE E'\b' CSV HEADER;

COPY cdm_source FROM './cdm_source.csv' CSV HEADER;

COPY condition_era FROM './condition_era.csv' CSV HEADER;

COPY condition_occurrence FROM './condition_occurrence.csv' CSV HEADER;

COPY death FROM './death.csv' CSV HEADER;

COPY drug_era FROM './drug_era.csv' CSV HEADER;

COPY drug_exposure FROM './drug_exposure.csv' CSV HEADER;

COPY location FROM './location.csv' CSV HEADER;

COPY measurement FROM './measurement.csv' CSV HEADER;

COPY observation_period FROM './observation_period.csv' CSV HEADER;

COPY observation FROM './observation.csv' CSV HEADER;

COPY person FROM './person.csv' QUOTE CSV HEADER;

COPY procedure_occurrence FROM './procedure_occurrence.csv' CSV HEADER;

COPY visit_occurrence FROM './visit_occurrence.csv' CSV HEADER;