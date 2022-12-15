FROM mcr.microsoft.com/azure-functions/dotnet-isolated:4-dotnet-isolated6.0
ENV AZURE_FUNCTIONS_ENVIRONMENT=Development
ENV AzureWebJobsScriptRoot=/home/site/wwwroot
ENV AzureFunctionsJobHost__Logging__Console__IsEnabled=true
ENV DomainEvents_Queue_QueueName="waterlootemplate-domain-events"
WORKDIR /home/site/wwwroot
COPY publish .
