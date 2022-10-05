# OMOP 5.3

The Hutch Agent currently only queries an OMOP Common Data Model (CDM) 5.x dataset.

## Structure

https://github.com/OHDSI/CommonDataModel

Follow the instructions / use the scripts for your preferred RDBMS Provider. The Hutch Stack is developed against PostgreSQL.

Versioned script archives, with per platform guidance, can be downloaded from the GitHub Releases.

Notes:

- the `5.3.1` Postgres DDL script is broken
  - find and replace `DATETIME2` with `TIMESTAMP` to fix.
  - https://github.com/OHDSI/CommonDataModel/issues/256
- When it says to populate data, if you have a dataset (see below), do so.

:::caution
For now, **do not** add foreign key constraints (see below).
:::

## Data

A sample dataset that can be used to test the Hutch stack in conjunction with a valid Activity Source (e.g. [BC|OS](/docs/users/detailed-overview/activity-sources#bcos-rquest)) is provided as a downloadable asset via a GitHub Release.

This [Sample Data only] release contains OMOP CDM 5.3 Sample Data.

:::caution
Because this dataset is all that is queried, there is no need to populate OMOP Vocabularies.

Because of this, you **should not** add the foreign key constraints to the schema.
:::

### Test Data issues / notes

- `location` schema mismatch (test vs 5.3.1 script)
  - extra `country`, `longitude` and `latitude` columns
    - ignore them
- `person` schema mismatch (test vs 5.3.1 script)
  - extra `death_datetime` column
    - ignore it

[Sample Data only]: https://github.com/hdruk/hutch/releases/tag/omop-5.3-sample-data
