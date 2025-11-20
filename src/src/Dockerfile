# =========================
# Build stage
# =========================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set working dir to /src so paths match repository layout when build context is the repo root (./src)
WORKDIR /src

# Copy solution and project file(s) with explicit paths so dotnet restore can find them regardless of build context
COPY ["src.sln", "./"]
COPY ["src/src.csproj", "src/"]

# Restore packages for the specific project
RUN dotnet restore "src/src.csproj"

# Copy the rest of the source
COPY . .

# Build and publish the project to /app/out
RUN dotnet publish "src/src.csproj" -c Release -o /app/out

# =========================
# Runtime stage
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copy published files from build stage
COPY --from=build /app/out .

# Expose port 8080 for Render (or mapping)
EXPOSE 8080

# Make ASP.NET Core listen on port 8080
ENV ASPNETCORE_URLS=http://+:8080

# Run the API using the compiled assembly
ENTRYPOINT ["dotnet", "src.dll"]
