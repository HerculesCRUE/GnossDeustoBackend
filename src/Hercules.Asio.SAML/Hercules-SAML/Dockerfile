FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /app
RUN ["dotnet", "restore"]
COPY . /app
RUN dotnet publish -c Release -o out
EXPOSE 5000
# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
COPY --from=build-env /app/out .
EXPOSE 5000
ENTRYPOINT ["dotnet", "Hercules.Asio.SAML.dll"]
