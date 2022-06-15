FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Sample.HotChocolate/Sample.HotChocolate.csproj", "Sample.HotChocolate/"]
RUN dotnet restore "Sample.HotChocolate/Sample.HotChocolate.csproj"
COPY . .
WORKDIR "/src/Sample.HotChocolate"
RUN dotnet build "Sample.HotChocolate.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Sample.HotChocolate.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Sample.HotChocolate.dll"]
