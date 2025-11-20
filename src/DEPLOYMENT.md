# Smart Resource Booking - Deployment Guide

## Docker Deployment

### Prerequisites
- Docker Desktop installed
- .NET 8.0 SDK (for local development)

### Build and Run with Docker

#### Option 1: Using docker-compose (Recommended)
```bash
# From the src directory
docker-compose up --build
```

#### Option 2: Using Docker commands directly
```bash
# Build the image (run from src directory)
docker build -t smart-resource-booking-api ./src

# Run the container
docker run -d -p 8080:8080 --name booking-api smart-resource-booking-api

# View logs
docker logs -f booking-api

# Stop the container
docker stop booking-api

# Remove the container
docker rm booking-api
```

### Access the Application
- API: http://localhost:8080
- Swagger UI: http://localhost:8080/swagger

### Environment Variables
You can customize the following environment variables:
- `ASPNETCORE_ENVIRONMENT`: Development, Staging, Production
- `ASPNETCORE_URLS`: URL binding (default: http://+:8080)

### Troubleshooting

#### Port already in use
If port 8080 is already in use, change it in docker-compose.yml:
```yaml
ports:
  - "9090:8080"  # Use port 9090 instead
```

#### Check container status
```bash
docker ps -a
```

#### View container logs
```bash
docker logs booking-api
```

#### Rebuild without cache
```bash
docker-compose up --build --force-recreate
```

### Production Deployment

For production, update the environment:
```bash
docker run -d -p 8080:8080 \
  -e ASPNETCORE_ENVIRONMENT=Production \
  --name booking-api \
  smart-resource-booking-api
```

## Database Configuration

Make sure to update your `appsettings.json` with the correct database connection string before deployment.

