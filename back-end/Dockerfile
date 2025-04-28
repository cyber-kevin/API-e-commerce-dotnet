FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

COPY ./RO.DevTest.WebApi/*.csproj ./RO.DevTest.WebApi/
COPY ./RO.DevTest.Application/*.csproj ./RO.DevTest.Application/
COPY ./RO.DevTest.Domain/*.csproj ./RO.DevTest.Domain/
COPY ./RO.DevTest.Infrastructure/*.csproj ./RO.DevTest.Infrastructure/
COPY ./RO.DevTest.Persistence/*.csproj ./RO.DevTest.Persistence/

RUN dotnet restore "./RO.DevTest.WebApi/RO.DevTest.WebApi.csproj"

COPY . .

WORKDIR /source/RO.DevTest.WebApi
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "RO.DevTest.WebApi.dll"]

