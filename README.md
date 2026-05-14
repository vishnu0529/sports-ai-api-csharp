![CI](https://github.com/vishnu0529/sports-ai-api-csharp/actions/workflows/ci.yml/badge.svg)
![.NET](https://img.shields.io/badge/.NET-10.0-purple)
![C#](https://img.shields.io/badge/C%23-ASP.NET%20Core-blue)
![JWT](https://img.shields.io/badge/Auth-JWT-orange)

# Sports AI Prediction API (C# / ASP.NET Core)

A professional REST API for AI-powered sports predictions, rebuilt from Python/FastAPI to C#/ASP.NET Core.

> Companion project to [sports-ai-api](https://github.com/vishnu0529/sports-ai-api) (Python/FastAPI version)

## Tech Stack
| Layer | Technology |
|---|---|
| Framework | ASP.NET Core 10 |
| Language | C# |
| Database | SQLite (local) / Azure SQL (production) |
| ORM | Entity Framework Core 10 |
| Authentication | JWT Bearer Tokens |
| Password Hashing | BCrypt |
| API Docs | Swagger / OpenAPI |
| Deployment | Azure App Service |
| CI/CD | GitHub Actions |

## API Endpoints

### Auth (Public)
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | /api/auth/register | Create a new account |
| POST | /api/auth/login | Login and get JWT token |

### Sport Events (Protected)
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | /api/sportevent | List all sport events |
| POST | /api/sportevent | Create a sport event |
| DELETE | /api/sportevent/{id} | Delete a sport event |

### Predictions (Protected)
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | /api/prediction | Get all my predictions |
| POST | /api/prediction | Submit a new prediction |

## Getting Started
```bash
git clone https://github.com/vishnu0529/sports-ai-api-csharp.git
cd sports-ai-api-csharp
dotnet restore
dotnet ef database update
dotnet run
```
Open Swagger UI: http://localhost:5139/swagger

## Roadmap
- [x] JWT Authentication
- [x] Sport Event endpoints
- [x] Prediction endpoints
- [x] Service layer architecture
- [ ] Global error handling middleware
- [ ] OpenAI integration
- [ ] Unit tests (xUnit)
- [ ] Azure App Service deployment

## Author
Vishnu — https://github.com/vishnu0529
