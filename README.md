# OpenFeature .NET Workshop: Le Mans Winners Management System

[![.NET 9.0](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![Aspire](https://img.shields.io/badge/Aspire-Enabled-purple)](https://learn.microsoft.com/en-us/dotnet/aspire/)
[![OpenFeature](https://img.shields.io/badge/OpenFeature-Ready-green)](https://openfeature.dev/)

A hands-on workshop demonstrating **OpenFeature** capabilities in a .NET environment. We will use a web application that stores the cars that we currently have in our collection.

## What You'll Learn

This workshop teaches you how to implement feature flags using OpenFeature in a real-world .NET application. You'll explore:

- Feature Flag Fundamentals: Toggle functionality without code deployments
- OpenFeature Integration: Industry-standard feature flagging for .NET
- Kill Switches: Safely disable features in production
- Dynamic Configuration: Modify application behavior at runtime
- A/B Testing: Experiment with user experiences
- Progressive Rollouts: Gradually release features to users

## Architecture

### Components

- Garage.Web: Blazor Server frontend for managing car collections
- Garage.ApiService: REST API for car data
- Garage.ServiceDefaults: Shared services including feature flag implementations
- Garage.Shared: Common models and DTOs
- Garage.AppHost: .NET Aspire orchestration and service discovery

## Feature Flags Included

The workshop demonstrates these feature flags:

| Flag                    | Type   | Purpose                         | Default |
| ----------------------- | ------ | ------------------------------- | ------- |
| `SlowOperationDelay`    | `int`  | Simulate processing delays      | 1000ms  |
| `EnableDatabaseWinners` | `bool` | Toggle data source (DB vs JSON) | `false` |
| `EnableStatsHeader`     | `bool` | Show/hide statistics header     | `true`  |

## Requirements

### Prerequisites

- .NET 9.0 SDK or later
- Visual Studio, Visual Studio Code with C# extension or JetBrains Rider
- Git for version control
- Docker Desktop (for containerized dependencies)

## Quick Start

### 1. Clone the Repository

```bash
git clone https://github.com/askpt/openfeature-dotnet-workshop.git
cd openfeature-dotnet-workshop
```

### 2. Restore Dependencies

```bash
dotnet restore
```

### 3. Run with .NET Aspire

```bash
cd src/Garage.AppHost
dotnet run
```

### 4. Access the Application

- Web Frontend: https://localhost:7070
- API Service: https://localhost:7071
- Aspire Dashboard: https://localhost:15888

Ready to start learning? Head over to [Workshop.md](Workshop.md) for step-by-step exercises that will guide you through.
Each exercise includes detailed instructions, expected outcomes, and troubleshooting tips to ensure a smooth learning experience.

## Additional Resources

- [OpenFeature Documentation](https://docs.openfeature.dev/)
- [.NET Aspire Documentation](https://learn.microsoft.com/en-us/dotnet/aspire/)
- [Feature Flag Best Practices](https://martinfowler.com/articles/feature-toggles.html)
- [Blazor Documentation](https://docs.microsoft.com/en-us/aspnet/core/blazor/)

## License

This project is licensed under the [MIT License](LICENSE).
