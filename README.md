<p>
  <a href="https://one-beyond.com">
    <img src="Logo.png" width="300" alt="One Beyond" />
  </a>
</p>

# Introduction

Obelisk is a WebAPI template built in ASP.Net and used in hundreds of succesful One Beyond projects. Obelisk in it's current .NET (Core) form has been in development and production use since early 2018 and draws on developments dating back to 2014 to accelerate delivery of ASP.Net MVC based projects. The core template is supported by core libraries and scaffolder utilities to accelerate development of SPA based front ends, e2e and load tests.

### Supported .NET version:

7.0

### Documentation

For more detailed documentation, please refer to our [Wiki](https://github.com/onebeyond/onebeyond-studio-obelisk/wiki)

### Contributing

If you want to contribute, we are currently accepting PRs and/or proposals/discussions in the issue tracker.


Obelisk is a WebAPI template built in ASP.Net and used in hundreds of succesful One Beyond projects. Obelisk in it's current .NET (Core) form has been in development and production use since early 2018 and draws on developments dating back to 2014 to accelerate delivery of ASP.Net MVC based projects. The core template is supported by core libraries and scaffolder utilities to accelerate development of SPA based front ends, e2e and load tests.

The primary motivation of the template solution (`“template”`) is having a standard starting point for all ASP.Net based WebAPI projects implemented by One Beyond Studio in terms of their architectural style, structure, and best practices. A key point is that the the template is not restrictive, but rather a non-opinionated guide where each part of a particular solution can be replaced by some other approach/implementation. Regardless, the template relies on and conveys some principles set in its core. The main ones are `Onion/Clean Architecture`, `Vertical Slices` and `CQS`. These architectural styles are reflected in the template structure, but at different levels. Why these three?

The `Onion/Clean Architecture` stems from the `Hexagonal Architecture` which aims at creating loosely coupled application components which can be exchanged at any level for the purposes of test automation. You can find representation of this approach in the template breakdown by projects, i.e., at its highest level. The template comprises of the following projects:
-	OneBeyond.Studio.Obelisk.Domain
-	OneBeyond.Studio.Obelisk.Application
-	OneBeyond.Studio.Obelisk.Infrastructure
-	OneBeyond.Studio.Obelisk.WebApi
-	OneBeyond.Studio.Obelisk.Workers

Each of the above projects is mapped to the corresponding layer of the `Onion/Clean Architecture`:
<p align="center">
    <img src="OnionArchitecture.png" width="700" alt="Onion or Clean Architecture" />
</p>


The `Vertical Slices` architecture, looks at the system from another perspective and focuses on the end-to-end implementation of a specific feature, grouping implementation details as close to each other as possible. Under each of the above projects you can find a folder called `Features` containing implementation of one or another vertical slice on a specific layer. Following this approach allows you to structure your system in a microservice like way with the only difference that all the services are hosted in the same process. This does have the added benefit of making future separation of services simpler.
<p align="center">
    <img src="VerticalSlices.png" width="700" alt="Feature Slice example" />
</p>

Any feature implementation is comprised of a set of commands and queries which can be executed against the system to obtain a desired behaviour, and this is where `CQS` comes into play. It allows you to implement handling of each request to the system in an isolated manner where each handler depends only on components required for processing the request. It binds action and reaction at a very granular level. For each feature implementation you can find a set of command and query handlers. The `CQS` should not be confused with `CQRS` which is a subset of the former. It makes use of command and query separation, but adds up a separation of data models, i.e., data affected by execution of commands lives separately from the data used for serving queries.
<p align="center">
    <img src="CQS.png" width="700" alt="High level write sides" />
</p>

Even though the entire concept outlines some layers it does not mean that it makes sense to have all of them all the time. One of the advantages of using `Vertical Slices` is that for implementing one feature you can go with all the layers in place, whereas for implementing the other you can go just with some of them due to the independent nature of the features.
