# =========================
# Build stage
# =========================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Use /src as working directory
WORKDIR /src

# Copy the entire build context into the image. This keeps the Dockerfile resilient
# regardless of whether the user runs `docker build` from repo root or from the `src` folder.
COPY . .

# Restore and publish: detect common csproj locations and use the first match.
# This supports both contexts:
#  - repo root (project path: src/src.csproj)
#  - src folder (project path: src.csproj)
RUN set -eux; \
    if [ -f "src/src.csproj" ]; then PROJECT_PATH="src/src.csproj"; \
    elif [ -f "src.csproj" ]; then PROJECT_PATH="src.csproj"; \
    elif [ -f "../src/src.csproj" ]; then PROJECT_PATH="../src/src.csproj"; \
    else echo "ERROR: .csproj not found in expected locations" >&2; exit 1; fi; \
    echo "Using project: $PROJECT_PATH"; \
    dotnet restore "$PROJECT_PATH"; \
    dotnet publish "$PROJECT_PATH" -c Release -o /app/out

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
