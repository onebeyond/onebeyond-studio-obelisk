var builder = DistributedApplication.CreateBuilder(args);

const string project = "OneBeyond.Studio.Obelisk";

var storage = builder
    .AddAzureStorage("storage")
    .RunAsEmulator(azurite =>
    {
        azurite
            .WithBlobPort(10000)
            .WithQueuePort(10001)
            .WithTablePort(10002);

        azurite.WithDataVolume($"{project}-storage-data-volume");
    });

var workers = builder
    .AddAzureFunctionsProject<Projects.OneBeyond_Studio_Obelisk_Workers>("workers")
    .WithHostStorage(storage);

var api = builder
    .AddProject<Projects.OneBeyond_Studio_Obelisk_WebApi>("api")
    .WithHttpHealthCheck("/health/live")
    .WaitFor(workers);

await builder.Build().RunAsync();
