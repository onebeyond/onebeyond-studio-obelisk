# Configuration Surface

Generated from `appsettings*.json`, `host.json`, and `local.settings.json`.

## `src/OneBeyond.Studio.Obelisk.AppHost/appsettings.Development.json`

- `Logging`: `Object`
- `Logging:LogLevel`: `Object`
- `Logging:LogLevel:Default`: `String`
- `Logging:LogLevel:Microsoft.AspNetCore`: `String`

## `src/OneBeyond.Studio.Obelisk.AppHost/appsettings.json`

- `Logging`: `Object`
- `Logging:LogLevel`: `Object`
- `Logging:LogLevel:Aspire.Hosting.Dcp`: `String`
- `Logging:LogLevel:Default`: `String`
- `Logging:LogLevel:Microsoft.AspNetCore`: `String`

## `src/OneBeyond.Studio.Obelisk.WebApi/appsettings.Development.json`

- `ClientApplication`: `Object`
- `ClientApplication:Url`: `String`
- `CookieAuthN`: `Object`
- `CookieAuthN:AllowCrossSiteCookies`: `True`
- `Cors`: `Object`
- `Cors:AllowedOrigins:Local`: `String`
- `DomainEvents`: `Object`
- `DomainEvents:Queue`: `Object`
- `DomainEvents:Queue:ConnectionString`: `String`
- `Identities`: `Object`
- `Identities:Seeding`: `Object`
- `Identities:Seeding:AdminPassword`: `String`
- `Jwt`: `Object`
- `Jwt:Secret`: `String`
- `Logging`: `Object`
- `Logging:LogLevel`: `Object`
- `Logging:LogLevel:Default`: `String`
- `Logging:LogLevel:Microsoft`: `String`
- `Logging:LogLevel:System`: `String`

## `src/OneBeyond.Studio.Obelisk.WebApi/appsettings.json`

- `AzureAd`: `Object`
- `AzureAd:CallbackPath`: `String`
- `AzureAd:ClientId`: `String`
- `AzureAd:Domain`: `String`
- `AzureAd:Instance`: `String`
- `AzureAd:Secret`: `Null`
- `AzureAd:SignedOutCallbackPath`: `String`
- `AzureAd:TenantId`: `String`
- `ClientApplication`: `Object`
- `ClientApplication:Url`: `String`
- `ConnectionStrings`: `Object`
- `ConnectionStrings:ApplicationConnectionString`: `String`
- `CookieAuthN`: `Object`
- `CookieAuthN:AllowCrossSiteCookies`: `False`
- `Cors`: `Object`
- `DomainEvents`: `Object`
- `DomainEvents:Queue`: `Object`
- `DomainEvents:Queue:ConnectionString`: `Null`
- `DomainEvents:Queue:QueueName`: `String`
- `EmailSender`: `Object`
- `EmailSender:Folder`: `Object`
- `EmailSender:Folder:EnforcedToEmailAddress`: `String`
- `EmailSender:Folder:Folder`: `String`
- `EmailSender:Folder:FromEmailAddress`: `String`
- `EmailSender:Folder:FromEmailName`: `String`
- `EmailSender:Folder:UseEnforcedToEmailAddress`: `False`
- `EmailSender:SendGrid`: `Object`
- `EmailSender:SendGrid:EnforcedToEmailAddress`: `String`
- `EmailSender:SendGrid:FromEmailAddress`: `String`
- `EmailSender:SendGrid:FromEmailName`: `String`
- `EmailSender:SendGrid:Key`: `Null`
- `EmailSender:SendGrid:UseEnforcedToEmailAddress`: `False`
- `FileStorage`: `Object`
- `FileStorage:AzureBlobStorage`: `Object`
- `FileStorage:AzureBlobStorage:ConnectionString`: `String`
- `FileStorage:AzureBlobStorage:ContainerName`: `String`
- `FileStorage:AzureBlobStorage:SharedAccessDuration`: `String`
- `FileStorage:FileSystem`: `Object`
- `FileStorage:FileSystem:StorageRootPath`: `String`
- `Identities`: `Object`
- `Identities:Seeding`: `Object`
- `Identities:Seeding:AdminPassword`: `Null`
- `Infrastructure`: `Object`
- `Infrastructure:EnableDetailedErrors`: `False`
- `Infrastructure:EnableSensitiveDataLogging`: `False`
- `Jwt`: `Object`
- `Jwt:AccessTokenExpirationMinutes`: `Number`
- `Jwt:Issuer`: `String`
- `Jwt:RefreshTokenExpirationDays`: `Number`
- `Jwt:Secret`: `Null`
- `KeyVault`: `Object`
- `KeyVault:Enabled`: `False`
- `KeyVault:Name`: `String`
- `Localization`: `Object`
- `Localization:SupportedCultures`: `Array`
- `Localization:SupportedCultures`: `Array[1]`
- `LockoutMaxFailedAccessAttempts`: `Number`
- `SecurityHeaders`: `Object`
- `SecurityHeaders:Access-Control-Expose-Headers`: `String`
- `SecurityHeaders:Strict-Transport-Security`: `String`
- `SecurityHeaders:X-Frame-Options`: `String`
- `SecurityHeaders:X-Permitted-Cross-Domain-Policies`: `String`
- `SecurityHeaders:X-XSS-Protection`: `String`

## `src/OneBeyond.Studio.Obelisk.WebApi/appsettings.QA.json`

- `ClientApplication`: `Object`
- `ClientApplication:Url`: `String`
- `CookieAuthN`: `Object`
- `CookieAuthN:AllowCrossSiteCookies`: `True`
- `Cors`: `Object`
- `Cors:AllowedOrigins:QA`: `String`
- `Logging`: `Object`
- `Logging:LogLevel`: `Object`
- `Logging:LogLevel:Default`: `String`
- `Logging:LogLevel:Microsoft.AspNetCore`: `String`
- `Logging:LogLevel:Microsoft.EntityFrameworkCore.Database.Command`: `String`

## `src/OneBeyond.Studio.Obelisk.Workers/appsettings.Development.json`

- `Logging`: `Object`
- `Logging:LogLevel`: `Object`
- `Logging:LogLevel:Default`: `String`
- `Logging:LogLevel:Microsoft`: `String`
- `Logging:LogLevel:System`: `String`

## `src/OneBeyond.Studio.Obelisk.Workers/appsettings.json`

- `ConnectionStrings`: `Object`
- `ConnectionStrings:ApplicationConnectionString`: `String`
- `EmailSender`: `Object`
- `EmailSender:Folder`: `Object`
- `EmailSender:Folder:EnforcedToEmailAddress`: `String`
- `EmailSender:Folder:Folder`: `String`
- `EmailSender:Folder:FromEmailAddress`: `String`
- `EmailSender:Folder:FromEmailName`: `String`
- `EmailSender:Folder:UseEnforcedToEmailAddress`: `False`
- `EmailSender:SendGrid`: `Object`
- `EmailSender:SendGrid:EnforcedToEmailAddress`: `String`
- `EmailSender:SendGrid:FromEmailAddress`: `String`
- `EmailSender:SendGrid:FromEmailName`: `String`
- `EmailSender:SendGrid:Key`: `Null`
- `EmailSender:SendGrid:UseEnforcedToEmailAddress`: `False`
- `Infrastructure`: `Object`
- `Infrastructure:EnableDetailedErrors`: `False`
- `Infrastructure:EnableSensitiveDataLogging`: `False`
- `KeyVault`: `Object`
- `KeyVault:Enabled`: `False`
- `KeyVault:Name`: `String`

## `src/OneBeyond.Studio.Obelisk.Workers/appsettings.QA.json`

- `Logging`: `Object`
- `Logging:LogLevel`: `Object`
- `Logging:LogLevel:Default`: `String`

## `src/OneBeyond.Studio.Obelisk.Workers/host.json`

- `logging`: `Object`
- `logging:applicationInsights`: `Object`
- `logging:applicationInsights:samplingSettings`: `Object`
- `logging:applicationInsights:samplingSettings:excludedTypes`: `String`
- `logging:applicationInsights:samplingSettings:isEnabled`: `True`
- `version`: `String`

## `src/OneBeyond.Studio.Obelisk.Workers/local.settings.json`

- `IsEncrypted`: `False`
- `Values`: `Object`
- `Values:AzureWebJobsStorage`: `String`
- `Values:DomainEvents_Queue_ConnectionString`: `String`
- `Values:DomainEvents_Queue_QueueName`: `String`
- `Values:FUNCTIONS_WORKER_RUNTIME`: `String`
- `Values:Jwt_Schedule`: `String`

