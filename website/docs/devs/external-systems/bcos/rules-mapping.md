---
sidebar_position: 3
---

# Rules mapping

This document describes how the rules in a BC|OS query map to tables in a target OMOP Data Source, in the contexts Hutch is interested in (which ultimately is everything a BC|OS query supports).

The information here was gleaned from a combination of BC Platforms documentation, conversations with their developers, and reverse engineering.

## Available tables

Here are the direct mappings from queryable targets in the BC|OS GUI to OMOP Tables.

| GUI Category | OMOP Table |
|-|-|
| Condition | `ConditionOccurrence` |
| Measurement | `Measurement` |
| Medication | `DrugExposure` |
| Observation | `Observation` |
| Person | `Person` |
| Procedure | `ProcedureOccurrence` |

## Rules

### Condition

The concept ID in `"value"` maps to the `condition_concept_id` column in `ConditionOccurrence`. 

In order to handle an age constraint, you need to join to `Person` table where `Person.birth_datetime` meets the time condition in `"time"`. In order to handle a time condition, you need to compare the `condition_start_date` column. To handle secondary modifiers, you need to look up the `condition_type_concept_id`.

#### With age/time condition

```json
// age condition

// age > 18 years
{
  "varname" : "OMOP",
  "type" : "TEXT",
  "oper" : "=",
  "value" : "4084167",
  "time" : "18|:AGE:Y",
  "seconady_modifier" : [ ]
}

// age < 18 years
{
  "varname" : "OMOP",
  "type" : "TEXT",
  "oper" : "=",
  "value" : "4084167",
  "time" : "|18:AGE:Y",
  "seconady_modifier" : [ ]
}

// time condition

// less than 18 months ago
{
  "varname" : "OMOP",
  "type" : "TEXT",
  "oper" : "=",
  "value" : "4084167",
  "time" : "|18:TIME:M",
  "seconady_modifier" : [ ]
}

// more than 18 months ago
{
  "varname" : "OMOP",
  "type" : "TEXT",
  "oper" : "=",
  "value" : "4084167",
  "time" : "18|:TIME:M",
  "seconady_modifier" : [ ]
}
```

#### With secondary modifiers

```json
{
  "varname" : "OMOP",
  "type" : "TEXT",
  "oper" : "=",
  "value" : "4084167",
  "seconady_modifier" : [ "32020" ]
}
```

#### With both age/time condition and secondary modifiers

```json
{
  "varname" : "OMOP",
  "type" : "TEXT",
  "oper" : "=",
  "value" : "4084167",
  "time" : "18|:TIME:M",
  "seconady_modifier" : [ "32020" ]
}
```

### Measurement

Measurements come as numeric rules (`"NUM"`) with an upper and/or lower bound in their `"value"` key. The upper and lower bounds can each be `null` or a decimal number (e.g. 10.0, 0.1, etc.). These values can be extracted and compared to the `value_as_number` column. The combinations or `null` and non-`null` bounds are shown below:

```json
// measurement with no lower or upper bound
// value_as_number = x
{
  "varname" : "OMOP=3006923",
  "type" : "NUM",
  "oper" : "=",
  "value" : "null..null"
}

// measurement with lower bound only
// value_as_number >= x
{
  "varname" : "OMOP=3006923",
  "type" : "NUM",
  "oper" : "=",
  "value" : "10..null"
}

// measurement with upper bound only
// value_as_number <= x
{
  "varname" : "OMOP=3006923",
  "type" : "NUM",
  "oper" : "=",
  "value" : "null..20"
}

// measurement with lower and upper bounds
// x <= value_as_number <= y
{
  "varname" : "OMOP=3006923",
  "type" : "NUM",
  "oper" : "=",
  "value" : "10..20"
}
```

### Medication, Observation, Person & Procedure

To query these tables without an age/time rule, you query `DrugExposure.drug_concept_id`, `Observation.observation_concept_id`, `Person.ethnicity_concept_id`, `Person.gender_concept_id`, `Person.race_concept_id` and `ProcedureOccurrence.procedure_concept_id`. 

In order to handle an age constraint, you need to join to `Person` table where `Person.birth_datetime` meets the time condition in `"time"`. To handle time constraints on Medication, compare the time value to `DrugExposure.drug_exposure_start_date`. For Observation, compare `Observation.observation_date`. For Procedure, compare `ProcedureOccurrence.procedure_date`.

```json
// without an age/time constraint
{
  "varname" : "OMOP",
  "type" : "TEXT",
  "oper" : "=",
  "value" : "3006923"
}

//with an age/time constraint
{
  "varname" : "OMOP",
  "type" : "TEXT",
  "oper" : "=",
  "value" : "4084167",
  "time" : "18|:AGE:Y"
}
```
