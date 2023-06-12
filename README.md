<p>
  <a href="https://one-beyond.com">
    <img src="Logo.png" width="300" alt="One Beyond" />
  </a>
</p>

[![License](https://img.shields.io/github/license/OneBeyond/onebeyond-studio-obelisk?style=plastic)](LICENSE)

# Introduction

Obelisk is a WebAPI template built in ASP.Net and used in hundreds of succesful One Beyond projects. Obelisk in it's current .NET (Core) form has been in development and production use since early 2018 and draws on developments dating back to 2014 to accelerate delivery of ASP.Net MVC based projects. The core template is supported by core libraries and scaffolder utilities to accelerate development of SPA based front ends, e2e and load tests.

### Supported .NET version:

7.0

### Setup

The project largely works out of the box when opening with a modern version of Visual Studio. The main pre-requisite is to have [Azurite](https://learn.microsoft.com/en-us/azure/storage/common/storage-use-azurite) running while running the project.

If you want to read any emails sent while working in a development environment, you will need to make sure the following path exists: `C:\inetpub\mailroot\Obelisk`. If not, then create it manually. Note: this only applies to Windows.

Finally, to run Obelisk, simply run the WebApi project through Visual Studio.

### Documentation

For more detailed documentation, please refer to our [Wiki](https://github.com/onebeyond/onebeyond-studio-obelisk/wiki)

### Contributing

If you want to contribute, we are currently accepting PRs and/or proposals/discussions in the issue tracker.
