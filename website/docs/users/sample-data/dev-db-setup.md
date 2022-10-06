# Setup Development OMOP Database

## Download the DDL for the OMOP CDM
Go to https://github.com/OHDSI/CommonDataModel/releases and select the version of the OMOP CDM that applies to you. We use OMOP CDM v5.3.2. Under "Assets", select the `OMOPCDM_*.zip` file, where `*` is the version number.

Unzipping the file, you'll get a folder called `output`. This contains sub-folders for each supported flavour of database. We use `postgresql`.

## Building the Database
In the `postgresql` folder, there are 4 scripts:
* `OMOPCDM_postgresql_5.3_ddl.sql`
  * the table definitions
* `OMOPCDM_postgresql_5.3_primary_keys.sql`
  * the primary keys for each table
* `OMOPCDM_postgresql_5.3_constraints.sql`
  * foreign keys, etc.
* `OMOPCDM_postgresql_5.3_indices.sql`
  * indices for each table

1. Run the DDL script on your database.
```bash
psql -h <db host> -d <db name> -U <username> -f OMOPCDM_postgresql_5.3_ddl.sql
# enter the password for your database when prompted
```

2. Add the primary keys.
```bash
psql -h <db host> -d <db name> -U <username> -f OMOPCDM_postgresql_5.3_primary_keys.sql
# enter the password for your database when prompted
```

3. Add the foreign keys and other constraints.
```bash
psql -h <db host> -d <db name> -U <username> -f OMOPCDM_postgresql_5.3_constraints.sql
# enter the password for your database when prompted
```

Before adding the indices, add the data to the database, as this is faster. Download the [sample data](https://github.com/HDRUK/hutch/releases/tag/omop-5.3-sample-data) and [`load_tables.sql`](https://github.com/HDRUK/hutch/blob/main/scripts/load_tables.sql) for loading the data to the tables you just created in the previous steps. Unzip the sample data `cd` into the unzipped directory. Then run:

4. Populate the tables in the database.
```bash
psql -h <db host> -d <db name> -U <username> -f load_tables.sql
# enter the password for your database when prompted
```

5. Apply the indices for faster querying.
```bash
psql -h <db host> -d <db name> -U <username> -f OMOPCDM_postgresql_5.3_indices.sql
# enter the password for your database when prompted
```
