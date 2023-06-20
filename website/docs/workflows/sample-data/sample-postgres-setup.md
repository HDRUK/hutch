# Sample PostgreSQL Database with OMOP 5.3

## Building the Database
There are 4 scripts to build the database:
1. `OMOPCDM_postgresql_5.3_ddl.sql`
    * the table definitions
2. `OMOPCDM_postgresql_5.3_primary_keys.sql`
    * the primary keys for each table
3. `OMOPCDM_postgresql_5.3_constraints.sql`
    * foreign keys, etc.
4. `OMOPCDM_postgresql_5.3_indices.sql`
    * indices for each table

You can run these scripts in order inside your favourite database software (e.g. DataGrip).

Alternatively you can run the following command using the `psql` CLI, substituting `<script>` for each of the above scripts:

```bash
psql -h <db host> -d <db name> -U <username> -f <script>
# enter the password for your database when prompted
```

## Loading the data
:::caution
Due to the size of some of the files in the sample data, you may run out of memory trying to load the data through your favourite database software.
:::

There is a script for loading the data called `load_tables_postgresql.sql` in the assets of the [sample data release](https://github.com/hdruk/hutch/releases/tag/omop-5.3-sample-data) on Github.

```bash
psql -h <db host> -d <db name> -U <username> -f load_tables_postgresql.sql
# enter the password for your database when prompted
```
