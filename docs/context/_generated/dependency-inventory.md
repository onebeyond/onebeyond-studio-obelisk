# Dependency Inventory

## Central package management

Versions are pinned in `Directory.Packages.props`.

- `Ardalis.SmartEnum.EFCore`: `8.2.0`
- `Asp.Versioning.Mvc.ApiExplorer`: `8.1.1`
- `Aspire.Hosting.Azure.Functions`: `13.1.0`
- `AspNetCore.HealthChecks.AzureStorage`: `7.0.0`
- `AspNetCore.HealthChecks.SqlServer`: `9.0.0`
- `AspNetCore.HealthChecks.UI.Client`: `9.0.0`
- `Autofac.Extensions.DependencyInjection`: `10.0.0`
- `AwesomeAssertions`: `9.3.0`
- `Azure.Monitor.OpenTelemetry.AspNetCore`: `1.4.0`
- `coverlet.collector`: `6.0.4`
- `Ensure.That`: `10.1.0`
- `Microsoft.ApplicationInsights.AspNetCore`: `2.23.0`
- `Microsoft.ApplicationInsights.WorkerService`: `2.23.0`
- `Microsoft.AspNetCore.Authentication.JwtBearer`: `10.0.1`
- `Microsoft.AspNetCore.Identity.EntityFrameworkCore`: `10.0.1`
- `Microsoft.AspNetCore.Mvc.Testing`: `10.0.1`
- `Microsoft.AspNetCore.OpenApi`: `10.0.2`
- `Microsoft.Azure.Functions.Worker`: `2.51.0`
- `Microsoft.Azure.Functions.Worker.Extensions.Storage.Queues`: `5.5.3`
- `Microsoft.Azure.Functions.Worker.Extensions.Timer`: `4.3.1`
- `Microsoft.Azure.Functions.Worker.Sdk`: `2.0.7`
- `Microsoft.EntityFrameworkCore.Design`: `10.0.1`
- `Microsoft.EntityFrameworkCore.Tools`: `10.0.1`
- `Microsoft.Extensions.Hosting.Abstractions`: `10.0.1`
- `Microsoft.Extensions.Http.Resilience`: `10.2.0`
- `Microsoft.Extensions.Identity.Stores`: `10.0.1`
- `Microsoft.Extensions.ServiceDiscovery`: `10.2.0`
- `Microsoft.Identity.Web`: `4.2.0`
- `Microsoft.NET.Test.Sdk`: `18.0.1`
- `Microsoft.VisualStudio.Web.CodeGeneration.Design`: `10.0.1`
- `Moq`: `4.20.72`
- `OneBeyond.Studio.Application.SharedKernel`: `10.0.0.7`
- `OneBeyond.Studio.Core.Mediator`: `10.0.0.7`
- `OneBeyond.Studio.DataAccess.EFCore`: `10.0.0.7`
- `OneBeyond.Studio.Domain.SharedKernel`: `10.0.0.7`
- `OneBeyond.Studio.EmailProviders.Domain`: `10.0.0.2`
- `OneBeyond.Studio.EmailProviders.Folder`: `10.0.0.2`
- `OneBeyond.Studio.EmailProviders.SendGrid`: `10.0.0.2`
- `OneBeyond.Studio.FileStorage.Azure`: `10.0.0.2`
- `OneBeyond.Studio.FileStorage.Domain`: `10.0.0.2`
- `OneBeyond.Studio.FileStorage.FileSystem`: `10.0.0.2`
- `OneBeyond.Studio.Hosting.AspNet`: `10.0.0.7`
- `OneBeyond.Studio.Infrastructure.Azure`: `10.0.0.7`
- `OneBeyond.Studio.TemplateRendering`: `10.0.0.1`
- `OpenTelemetry.Exporter.OpenTelemetryProtocol`: `1.15.0`
- `OpenTelemetry.Extensions.Hosting`: `1.15.0`
- `OpenTelemetry.Instrumentation.AspNetCore`: `1.15.0`
- `OpenTelemetry.Instrumentation.Http`: `1.15.0`
- `OpenTelemetry.Instrumentation.Runtime`: `1.15.0`
- `Swashbuckle.AspNetCore.SwaggerUI`: `10.1.2`
- `System.IdentityModel.Tokens.Jwt`: `8.15.0`
- `xunit`: `2.9.3`
- `xunit.runner.visualstudio`: `3.1.5`

## Project package references

### `OneBeyond.Studio.Obelisk.AppHost`

- Package: `Aspire.Hosting.Azure.Functions`
- Project reference: `../OneBeyond.Studio.Obelisk.WebApi/OneBeyond.Studio.Obelisk.WebApi.csproj`
- Project reference: `../OneBeyond.Studio.Obelisk.Workers/OneBeyond.Studio.Obelisk.Workers.csproj`

### `OneBeyond.Studio.Obelisk.Application`

- Package: `OneBeyond.Studio.Application.SharedKernel`
- Package: `OneBeyond.Studio.EmailProviders.Domain`
- Package: `OneBeyond.Studio.TemplateRendering`
- Project reference: `../OneBeyond.Studio.Obelisk.Authentication/OneBeyond.Studio.Obelisk.Authentication.Domain/OneBeyond.Studio.Obelisk.Authentication.Domain.csproj`
- Project reference: `../OneBeyond.Studio.Obelisk.Domain/OneBeyond.Studio.Obelisk.Domain.csproj`

### `OneBeyond.Studio.Obelisk.Authentication.Application`

- Package: `Microsoft.AspNetCore.Authentication.JwtBearer`
- Package: `Microsoft.Extensions.Hosting.Abstractions`
- Package: `Microsoft.Extensions.Identity.Stores`
- Package: `Microsoft.Identity.Web`
- Package: `OneBeyond.Studio.Application.SharedKernel`
- Package: `OneBeyond.Studio.Domain.SharedKernel`
- Package: `System.IdentityModel.Tokens.Jwt`
- Project reference: `../OneBeyond.Studio.Obelisk.Authentication.Domain/OneBeyond.Studio.Obelisk.Authentication.Domain.csproj`

### `OneBeyond.Studio.Obelisk.Authentication.Domain`

- Package: `Ensure.That`
- Package: `OneBeyond.Studio.Core.Mediator`

### `OneBeyond.Studio.Obelisk.Domain.Tests`

- Project reference: `../OneBeyond.Studio.Obelisk.Domain/OneBeyond.Studio.Obelisk.Domain.csproj`

### `OneBeyond.Studio.Obelisk.Domain`

- Package: `OneBeyond.Studio.Domain.SharedKernel`
- Package: `OneBeyond.Studio.FileStorage.Domain`

### `OneBeyond.Studio.Obelisk.Infrastructure`

- Package: `Ardalis.SmartEnum.EFCore`
- Package: `Microsoft.AspNetCore.Identity.EntityFrameworkCore`
- Package: `Microsoft.EntityFrameworkCore.Tools`
- Package: `OneBeyond.Studio.Application.SharedKernel`
- Package: `OneBeyond.Studio.DataAccess.EFCore`
- Project reference: `../OneBeyond.Studio.Obelisk.Application/OneBeyond.Studio.Obelisk.Application.csproj`
- Project reference: `../OneBeyond.Studio.Obelisk.Authentication/OneBeyond.Studio.Obelisk.Authentication.Application/OneBeyond.Studio.Obelisk.Authentication.Application.csproj`

### `OneBeyond.Studio.Obelisk.ServiceDefaults`

- Package: `Azure.Monitor.OpenTelemetry.AspNetCore`
- Package: `Microsoft.Extensions.Http.Resilience`
- Package: `Microsoft.Extensions.ServiceDiscovery`
- Package: `OpenTelemetry.Exporter.OpenTelemetryProtocol`
- Package: `OpenTelemetry.Extensions.Hosting`
- Package: `OpenTelemetry.Instrumentation.AspNetCore`
- Package: `OpenTelemetry.Instrumentation.Http`
- Package: `OpenTelemetry.Instrumentation.Runtime`

### `OneBeyond.Studio.Obelisk.WebApi.Tests`

- Package: `AwesomeAssertions`
- Package: `Microsoft.AspNetCore.Mvc.Testing`
- Package: `Moq`
- Project reference: `../OneBeyond.Studio.Obelisk.WebApi/OneBeyond.Studio.Obelisk.WebApi.csproj`

### `OneBeyond.Studio.Obelisk.WebApi`

- Package: `Asp.Versioning.Mvc.ApiExplorer`
- Package: `AspNetCore.HealthChecks.AzureStorage`
- Package: `AspNetCore.HealthChecks.SqlServer`
- Package: `AspNetCore.HealthChecks.UI.Client`
- Package: `Autofac.Extensions.DependencyInjection`
- Package: `Microsoft.AspNetCore.OpenApi`
- Package: `Microsoft.EntityFrameworkCore.Design`
- Package: `Microsoft.VisualStudio.Web.CodeGeneration.Design`
- Package: `OneBeyond.Studio.EmailProviders.Folder`
- Package: `OneBeyond.Studio.EmailProviders.SendGrid`
- Package: `OneBeyond.Studio.FileStorage.Azure`
- Package: `OneBeyond.Studio.FileStorage.FileSystem`
- Package: `OneBeyond.Studio.Hosting.AspNet`
- Package: `OneBeyond.Studio.Infrastructure.Azure`
- Package: `Swashbuckle.AspNetCore.SwaggerUI`
- Project reference: `../OneBeyond.Studio.Obelisk.Infrastructure/OneBeyond.Studio.Obelisk.Infrastructure.csproj`
- Project reference: `../OneBeyond.Studio.Obelisk.ServiceDefaults/OneBeyond.Studio.Obelisk.ServiceDefaults.csproj`

### `OneBeyond.Studio.Obelisk.Workers`

- Package: `Autofac.Extensions.DependencyInjection`
- Package: `Microsoft.Azure.Functions.Worker`
- Package: `Microsoft.Azure.Functions.Worker.Extensions.Storage.Queues`
- Package: `Microsoft.Azure.Functions.Worker.Extensions.Timer`
- Package: `Microsoft.Azure.Functions.Worker.Sdk`
- Package: `OneBeyond.Studio.EmailProviders.Folder`
- Package: `OneBeyond.Studio.EmailProviders.SendGrid`
- Package: `OneBeyond.Studio.Infrastructure.Azure`
- Project reference: `../OneBeyond.Studio.Obelisk.Authentication/OneBeyond.Studio.Obelisk.Authentication.Application/OneBeyond.Studio.Obelisk.Authentication.Application.csproj`
- Project reference: `../OneBeyond.Studio.Obelisk.Infrastructure/OneBeyond.Studio.Obelisk.Infrastructure.csproj`
- Project reference: `../OneBeyond.Studio.Obelisk.ServiceDefaults/OneBeyond.Studio.Obelisk.ServiceDefaults.csproj`

