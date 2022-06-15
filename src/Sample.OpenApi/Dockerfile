FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Sample.OpenApi/Sample.OpenApi.csproj", "Sample.OpenApi/"]
RUN dotnet restore "Sample.OpenApi/Sample.OpenApi.csproj"
COPY . .
WORKDIR "/src/Sample.OpenApi"
RUN dotnet build "Sample.OpenApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Sample.OpenApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Sample.OpenApi.dll"]
