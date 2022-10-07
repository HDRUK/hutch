# OMOP 5.3

The Hutch Agent currently only queries an OMOP Common Data Model (CDM) 5.x dataset.

## Structure

The SQL scripts to build an OMOP database can be found at https://github.com/OHDSI/CommonDataModel/releases. Select the version of the OMOP CDM that applies to you. Under "Assets", select the `OMOPCDM_*.zip` file, where `*` is the version number.

Unzipping the file, you'll get a folder called `output`. This contains sub-folders for each supported flavour of database (e.g. PostgreSQL, MySQL, etc.).

Follow the instructions below to install for your database flavour:
* [PostgreSQL](dev-db-setup.md)


## Sample Data

A sample dataset that can be used to test the Hutch stack in conjunction with a valid Activity Source (e.g. [BC|OS](/docs/users/detailed-overview/activity-sources#bcos-rquest)) is provided as a downloadable asset via a GitHub Release.

This [Sample Data only] release contains OMOP CDM 5.3 Sample Data.

### Test Data issues / notes

- `location` schema mismatch (test vs 5.3.1 script)
  - extra `country`, `longitude` and `latitude` columns
    - ignore them
- `person` schema mismatch (test vs 5.3.1 script)
  - extra `death_datetime` column
    - ignore it

[Sample Data only]: https://github.com/hdruk/hutch/releases/tag/omop-5.3-sample-data
