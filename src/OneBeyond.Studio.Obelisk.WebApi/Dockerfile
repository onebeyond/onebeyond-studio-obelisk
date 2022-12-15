FROM mcr.microsoft.com/dotnet/aspnet:6.0
EXPOSE 80
EXPOSE 443
ENV ASPNETCORE_ENVIRONMENT=Development
WORKDIR /app
COPY publish .
ENTRYPOINT ["dotnet", "WaterlooTemplate.WebApi.dll"]
