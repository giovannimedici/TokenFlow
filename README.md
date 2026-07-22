# TokenFlow API

TokenFlow is a .NET 8 minimal API for user registration and JWT-based authentication. User data is stored in MongoDB, passwords are hashed with BCrypt, and generated tokens expire after 30 minutes.

## Features

- Create users with password-strength validation
- Authenticate users and receive a JWT
- Persist users in MongoDB
- Validate requests with FluentValidation
- Expose Swagger UI in the Development environment
- Provide a health-check endpoint
- Run tests in CI with GitHub Actions
- Run in Docker or deploy to Fly.io

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- A MongoDB instance or MongoDB Atlas cluster
- Docker (optional)

## Configuration

The API requires a MongoDB connection string, database name, and JWT signing key. Configure them through environment variables—do not commit production credentials to the repository.

```bash
export MongoDB__ConnectionURI="mongodb://localhost:27017"
export MongoDB__DatabaseName="TokenFlow"
export Jwt__Key="replace-with-a-long-random-secret"
```

For local development, you may instead use `src/TokenFlow.API/appsettings.Development.json`. Keep this file free of real credentials or exclude it from version control.

## Run locally

```bash
dotnet restore
dotnet run --project src/TokenFlow.API
```

When running in the Development environment, Swagger UI is available at:

```text
https://localhost:<port>/swagger
```

The port is defined by the launch profile or command output.

## API endpoints

### Health check

```http
GET /health
```

Response:

```json
{ "status": "ok" }
```

### Create a user

```http
POST /users
Content-Type: application/json
```

```json
{
  "username": "john",
  "password": "Secret123!"
}
```

Successful response (`201 Created`):

```json
{
  "id": "00000000-0000-0000-0000-000000000000",
  "username": "john"
}
```

Username requirements:

- 3 to 50 characters
- Letters and numbers only

Password requirements:

- 8 to 50 characters
- At least one uppercase letter, lowercase letter, number, and special character

### Authenticate

```http
POST /users/authenticate
Content-Type: application/json
```

```json
{
  "username": "john",
  "password": "Secret123!"
}
```

Successful response (`200 OK`) is the JWT string:

```json
"eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
```

## Error responses

- `400 Bad Request`: request validation failed
- `422 Unprocessable Entity`: domain rule violation, such as an existing username or invalid credentials
- `500 Internal Server Error`: unexpected server error

## Tests

```bash
dotnet test
```

## Docker

Build and run the API:

```bash
docker build -t tokenflow-api .
docker run --rm -p 8080:8080 \
  -e MongoDB__ConnectionURI="mongodb://host.docker.internal:27017" \
  -e MongoDB__DatabaseName="TokenFlow" \
  -e Jwt__Key="replace-with-a-long-random-secret" \
  tokenflow-api
```

The container listens on port `8080`.
