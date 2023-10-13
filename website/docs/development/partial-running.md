It's possible to run Hutch with only partial engagement with the external services it interacts with.

This document provides notes around how to configure Hutch to skip certain external interactions, which can simplify development setup and actually developing and testing certain areas of the application.

Obviously features should be fully tested with all external services before being considered complete and working, but omitting some interactions can significantly quicken the internal developer loop.

## Dummy Controller API

Hutch provides a dummy implementation of the TRE Controller API which is interface compliant with only the parts of the TRE Controller that Hutch interacts with.

You can run this in development as a substitute for a real Controller API implementation, but it still requires OIDC config.

## Standalone Mode

Standalone mode forces Hutch to run without ever interacting with a TRE Controller API (not even the dummy one).

This means you don't need a TRE Controller API.

It also means (depending on your Intermediary Store config) that you may not need OIDC config (and therefore not need an Identity Provider like Keycloak). See [below](#intermediary-store-without-oidc) 

In Standalone mode, Hutch does the following things:

- Status Updates only log to local logging targets - no HTTP requests are made
- InitiateEgress will not make an HTTP request for Egress Bucket Details; instead it will substitute the locally configured details in "StoreDefaults"
- InitiateEgress will not make an HTTP request for `FilesReadyForReview` when it has uploaded outputs to the Egress Bucket.
  - it will log when output uploading is complete, and advise you to make an approval request manually
- It will not make a `FinalOutcome` HTTP Request once packaging and upload of the final results crate is complete

Changes to interacting with Hutch are therefore as follows:

- You must manually submit jobs to the `/jobs` endpoint
- You must manually approve egress at the `/jobs/{id}/approval` endpoint

## Intermediary Store without OIDC

In the full TRE-FX stack, the expectation is that Hutch will interact with an OIDC Identity Provider to get tokens, and those tokens will be used in two places:

- all TRE Controller API requests
- to get temporary credentials for Intermediary Store API requests

It is however possible to use the Intermediary Store without OIDC.

If Hutch's "StoreDefaults" contain an `accessKey` and `secretKey` these can be used instead of using an OIDC token to get temporary ones.

Also for job submissions, the `crateSource` can include an `accessKey` and `secretKey` to be used directly.

If Hutch is also in Standalone Mode, then OIDC is not required at all, and Hutch's OIDC configuration can be omitted, and the OIDC service (e.g. Keycloak in the development `docker-compose`) need not be run or configured.

## Skip Workflow Execution

When Hutch receives a job, it fetches a referenced workflow for the job and executes it, the handles returning the outputs to the job's source via the Intermediary Store.

However this requires environment setup of the actual Workflow Executor (e.g. Wfexs), and workflow staging and execution can be quite slow.

It is possible to skip execution altogether (but retain the integrity of the rest of the job lifecycle - before and after execution), which can be useful when developing or testing post-execution behaviours.

To do this, you'll need a suitable zip file to act as the outputs from the execution that never happened. Hutch will then substitute this output file at the correct point in the lifecycle, and carry on as if everything was normal.

It's recommended to use a real execution output if possible - even better if it's for the input job's workflow, to at least appear to be authentic.

Set the setting `WorkflowExecutor:SkipExecutionWithOutputFile` to a non-empty value, which should be the path to your dummy output zip. Absolute paths are fine; relative paths are relative to the working directory root for Hutch as defined in the setting `Paths:WorkingDirectoryBase`. It's intended for this file to be used statically across multiple job runs, so it's not a per-job path.
