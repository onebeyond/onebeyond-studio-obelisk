<p>
  <a href="https://one-beyond.com">
    <img src="Logo.png" width="300" alt="One Beyond" />
  </a>
</p>

[![License](https://img.shields.io/github/license/OneBeyond/onebeyond-studio-obelisk?style=plastic)](LICENSE) [![Maintainability](https://api.codeclimate.com/v1/badges/f8e43de616f1c08ee399/maintainability)](https://codeclimate.com/github/onebeyond/onebeyond-studio-obelisk/maintainability) [![Test Coverage](https://api.codeclimate.com/v1/badges/f8e43de616f1c08ee399/test_coverage)](https://codeclimate.com/github/onebeyond/onebeyond-studio-obelisk/test_coverage)

# Introduction

Obelisk is a WebAPI template built in ASP.Net and used in hundreds of succesful One Beyond projects. Obelisk in its current .NET (Core) form has been in development and production use since early 2018 and draws on developments dating back to 2014 to accelerate delivery of ASP.Net MVC based projects. The core template is supported by core libraries and scaffolder utilities to accelerate development of SPA based frontends, e2e and load tests.

### Supported .NET version:

10.0

### Setup

Obelisk solution uses Directory.Build.props (central package management) for managing common project properties. The framework targets and language versions are specified there and used by all projects within a solution.  

Also, Obelisk uses Aspire to run the solution, which consists of:
- Web API to provide API endpoints to external clients
- Workers, based on Azure functions, to handle background processes (like, domain events handling and scheduled tasks)
- Azure storage (based on which Obelisk domain envents handling is implemented)

To run it, please install:
1. [Docker desktop](https://docs.docker.com/desktop/) (Aspire spins up a Docker container for the Azurite storage with a persistent backend)
2. [Azure Functions Core Tools](https://learn.microsoft.com/en-us/azure/azure-functions/functions-run-local)
3. and also make sure that Azure functions and toolsets are up to date. In Visual Studio `Tools`->`Options`-> `Azure Functions` <img width="767" height="181" alt="image" src="https://github.com/user-attachments/assets/82907921-91e9-4ddd-9e21-93f389f74a67" />


The default implementation of IEmailSender used in the application will store all generated e-mails in the following folder: `C:\inetpub\mailroot\Obelisk` (you can find the related code [here](https://github.com/onebeyond/onebeyond-studio-obelisk/blob/main/src/OneBeyond.Studio.Obelisk.WebApi/Program.cs#LL148C15-L148C15)). This folder can be configured in [`appsettings.json -> EmailSender -> Folder -> Folder` section](https://github.com/onebeyond/onebeyond-studio-obelisk/blob/main/src/OneBeyond.Studio.Obelisk.WebApi/appsettings.json#L33). Please create this folder manually if it does not exist. Note: this only applies to Windows.

To run the solution in VS, select the AppHost as startup project and press run. Can also `dotnet run` in the AppHost project.

<img width="1225" height="379" alt="image" src="https://github.com/user-attachments/assets/132c8870-fe1b-4c29-ba9b-ac2f2963d68a" />

### Documentation

For more detailed documentation, please refer to our [Wiki](https://github.com/onebeyond/onebeyond-studio-obelisk/wiki)

### Contributing

If you want to contribute, we are currently accepting PRs and/or proposals/discussions in the issue tracker.
