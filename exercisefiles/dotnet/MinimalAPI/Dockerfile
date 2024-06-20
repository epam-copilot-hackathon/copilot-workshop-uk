# create dotnet 7 image for the current project

FROM mcr.microsoft.com/dotnet/sdk AS build
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.csproj .
RUN dotnet restore

# Copy everything else and build the project
COPY . .
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "MinimalAPI.dll"]