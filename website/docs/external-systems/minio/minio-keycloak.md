You can connect Minio to Keycloak as an additional authentication source.

The `docker-compose` gives an example of this setup.

You'll also need to do some setup within Keycloak (detailed below) so that Minio can authenticate and have the right scopes/claims etc.

Note that this setup is using the **JWT Claim** approach detailed [here][Minio JWT Claim], *not* the **RolePolicy** approach from the same page.

This aligns with the current behaviour expected by the rest of the TRE-FX stack.

If this is done and Hutch is expected to get credentials for MinIO with an OIDC token, Hutch **MUST** use MinIO's client credentials to request the token, not a different client. This is because MinIO checks the `azp` claim in the token and expects it to match its own OIDC client-id, otherwise:

```xml
<ErrorResponse xmlns="https://sts.amazonaws.com/doc/2011-06-15/">
  <Error>
    <Type></Type>
    <Code>InvalidParameterValue</Code>
    <Message>
    STS JWT Token has `azp` claim invalid, `azp` must match configured OpenID Client ID
    </Message>
  </Error>
  <RequestId>178D5E53CEF44BA1</RequestId>
</ErrorResponse>
```

This is kinda infuriating as really `azp` should be the token requester (Hutch) and `aud` should be the token consumer(s) (arguably Hutch but definitely MinIO).

## Create a Realm

If not already created, follow the instructions on creating a Realm in the Keycloak section.

It should match the realm name used in Minio's OpenID Config URL setting, e.g. `hutch-dev` for the development `docker-compose`. 

## Create a Client for Minio (and Hutch!)

General Settings:

- type: `OpenID Connect`
- client id: should match Minio config.
  - The development `docker-compose` uses `minio`

Capability Config:

- Client Authentication: `Off` (**for development**)
  - If you set this to `On` clients must provide a secret
  - You typically want this in production
  - but for the development `docker-compose` it's easier to leave it `Off`
  - so that MinIO (and Hutch!) doesn't need configuring with the secret that you can't generate until keycloak is already running
- Authentication Flow:
  - [x] Standard flow
    - This is OIDC's "Authorization Code Flow" and is the only flow minio supports
  - [x] Direct access grants
    - This is OIDC's "Resource Owner Password Credentials Grant" and is currently all Hutch supports, because MinIO and the TRE Controller API expect user tokens, not client ones.

Login Settings:

- Root URL should be Minio's configured URL e.g. for development `http://host.docker.internal:9001`
- Most other settings are URLs and for development can just be wildcarded with `*`

## Create (a) User(s)

Minio's OIDC auth is wholly user-centric, so you will need users (at least one) in Keycloak.

## Grant Users Minio policy claims

Minio will only authenticate OIDC Users that have a `policy` claim containing one or more policies for Minio to apply to the users, as defined [here][Minio Policies].

To configure this in Keycloak you use "Mappers", which map from Keycloak data to token data.

There are a number of ways to configure Keycloak to do this, largely beyond the scope of this documentation.

What we will do for development purposes is modify an existing default Client Scope to always add a default policy claim, rather than per user or role. If we do it on a scope like `profile` that gets included in the identity token by default, Minio (and other clients!) can pick it up automatically.

To do this:

1. Go to `Client scopes` in the left-hand nav
1. Go to the `profile` scope.
1. On the `Mappers` tab, choose `Add mapper` -> `By Configuration`
1. Choose the type `Hardcoded claim`
  - in Production you might instead map a per-User attribute here
1. Add a friendly name of your choice for the Mapper, possibly aligned with the claim it maps - e.g. `policy`
1. Token Claim Name should be `policy` to match Minio's default expectation
1. Claim value should be one or more [valid Minio Policies][Minio Policies]
  - `consoleAdmin` is a simple choice **for development only**
  - this value determines what rights within Minio the user(s) will be granted.
1. ensure `Add to id token` is `On` - we want this claim in id tokens as Minio needs it.
1. Save

[Minio Policies]: https://min.io/docs/minio/linux/administration/identity-access-management/policy-based-access-control.html#minio-policy
[Minio JWT Claim]: https://min.io/docs/minio/linux/administration/identity-access-management/oidc-access-management.html#json-web-token-claim
