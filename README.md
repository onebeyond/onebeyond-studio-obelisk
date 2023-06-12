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

The project largely works out of the box when opening with a modern version of Visual Studio. 

Please note, Obelisk architecture uses Azure queues to dispatch domain events, to run the solution localy you'll need to have [Azurite](https://learn.microsoft.com/en-us/azure/storage/common/storage-use-azurite) running.

Also, the default implementation of IEmailSender used in the application will store all generated e-mails in the following folder:`C:\inetpub\mailroot\Obelisk` (you can find the related code [here](https://github.com/onebeyond/onebeyond-studio-obelisk/blob/main/src/OneBeyond.Studio.Obelisk.WebApi/Program.cs#LL148C15-L148C15)). Please create this folder manually if it does not exist. Note: this only applies to Windows.

Finally, to run Obelisk, simply run the WebApi project through Visual Studio.

### Documentation

For more detailed documentation, please refer to our [Wiki](https://github.com/onebeyond/onebeyond-studio-obelisk/wiki)

### Contributing

If you want to contribute, we are currently accepting PRs and/or proposals/discussions in the issue tracker.
