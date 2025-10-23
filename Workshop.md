# OpenFeature .NET Workshop Exercises

Welcome to the OpenFeature .NET Workshop! This hands-on guide will take you through implementing feature flags using
OpenFeature in a real-world .NET application.

## Prerequisites

Before starting, make sure you have:

- Completed the setup instructions in the [README](README.md)
- The application running via .NET Aspire
- Access to the Aspire Dashboard at <https://localhost:15888>
- Make sure the Docker containers are running

## Workshop Flow

Each exercise builds upon the previous one, teaching you progressively more advanced OpenFeature concepts. Take your
time with each exercise and don't hesitate to experiment!

---

## Exercise 1: Basic Feature Flags

**Goal**: Implement your first feature flag (without OpenFeature) and understand the basics

### What You'll Learn

- How feature flags work in the application
- Basic boolean flag implementation

### Tasks

1. Explore the Current Implementation

   - Navigate to `src/Garage.ServiceDefaults/Services/IFeatureFlags.cs`
   - Review the `EnableStatsHeader` flag interface
   - Look at `FeatureFlags.cs` to see the environment variable implementation

2. Toggle the Statistics Header

   - Locate the environment variables in `src/Garage.AppHost/Properties/launchSettings.json`
   - Change the `EnableStatsHeader` environment variable from `"true"` to `"false"`
   - Restart the application to see the behavior change in the web frontend
   - Toggle it back to `"true"`

3. Verify the Changes

   - Navigate to the web frontend
   - Confirm the statistics header appears/disappears based on your flag value

4. Create a New Feature Flag

   - Add a new boolean flag in `IFeatureFlags.cs` (`EnableTabs`) making sure it is `true`
   - Implement it in `FeatureFlags.cs` to read from environment variable `EnableTabs`
   - Add the environment variable to `launchSettings.json` with value `"true"`
   - Use this flag in the `.collection-tabs` div in the `Home.razor` file. Tip: You can use the `ShowHeader` flag as a reference

### Expected Outcome

You should see the statistics header toggle on and off based on your feature flag value.

### Learning Outcomes

- Understand that feature flags are an application development practice that can be implemented basically (but as we'll see, can also become quite powerful and complex).
- Learn how environment variables can be used to configure feature flags at application startup.

**Note**: In this initial exercise, feature flags are configured using environment variables in the `launchSettings.json` file. Changes require an application restart. Later exercises will introduce dynamic flag updates through OpenFeature.

---

## Exercise 2: Introduce OpenFeature

**Goal**: Use OpenFeature to manage feature flags dynamically and adding a custom provider

### What You'll Learn

- How to integrate OpenFeature into a .NET application
- Implementing a custom feature flag provider

### Tasks

1. Install OpenFeature Dependencies

   ```bash
   dotnet add src/Garage.ServiceDefaults package OpenFeature.Hosting
   ```

2. Configure OpenFeature in Extensions file

   - Open `src/Garage.ServiceDefaults/Extensions.cs`
   - Add OpenFeature services in the `AddFeatureFlags` method

3. Implement a Custom Provider

   - Create a new class `CustomFeatureProvider` in `src/Garage.ServiceDefaults/Providers`
   - Look at the `FeatureProvider` base class. Tip: You can try access the environment variables directly in the `CustomFeatureProvider` class. Tip: You can skip implementing the `ResolveStructureValueAsync` method for now
   - Add the provider to the OpenFeature configuration in `Extensions.cs`

4. Replace the IFeatureFlags dependency injection

   - Modify the usages of `IFeatureFlags` interface to use OpenFeature `IFeatureClient` instead

5. Make sure to use the UserId in the OpenFeature context

   - Use the `_userId` property in the `Home.razor` file to set the user context for OpenFeature
   - Ensure the OpenFeature client is aware of the user context. Tip: Have a look at the `EvaluationContext` class.

### Expected Outcome

You will have a basic OpenFeature integration that allows you to manage feature flags dynamically. You should
see the custom provider in action, and the feature flags should be evaluated based on the user context.

### Learning Outcomes

- Understand that OpenFeature can be powered by any underlying "backend", as long as it can resolve feature flag values.

---

## Exercise 3: Add flagd provider

**Goal**: Integrate the `flagd` provider for feature flags

### What You'll Learn

- How to use an external feature flag service
- Configuring OpenFeature with `flagd`
- Real-time flag updates without application restarts

### Tasks

1. Install the flagd Provider

   ```bash
   dotnet add src/Garage.ServiceDefaults package OpenFeature.Contrib.Providers.Flagd
   ```

2. Configure the flagd Provider

   - Open `src/Garage.ServiceDefaults/Extensions.cs`
   - Add the `FlagdProvider` to the OpenFeature configuration

3. Set Up flagd

   - Open `src/Garage.AppHost/Program.cs`
   - Configure the `FlagdProvider` with the correct endpoint

   ```csharp
   var flagd = builder.AddContainer("flagd", "ghcr.io/open-feature/flagd:latest")
       .WithBindMount("flags","/flags")
       .WithArgs("start", "--uri", "file:./flags/flagd.json")
       .WithEndpoint(8013, 8013);
   ```

4. Configure the flagd dependents

   - Open `src/Garage.AppHost/Program.cs`
   - Modify the `apiservice` and `webfrontend` services to wait for the `flagd` service

### Expected Outcome

You will have integrated the `flagd` provider into your OpenFeature setup, allowing you to manage feature flags
dynamically using an external service. You should be able to modify flags in the `flagd.json` file and see the changes
reflected in real-time without restarting the application.

### Learning Outcomes

- Understand that flagd is an OpenFeature-compatible backend for feature flags

---

## Exercise 4: Performance Tuning with Integer Flags

**Goal**: Use integer flags to control performance characteristics

### What You'll Learn

- How integer flags can control performance
- Real-time impact of performance tuning
- Using flags to simulate different performance scenarios

### Tasks

1. Understand the SlowOperationDelay Flag

   - Find where this delay is implemented in the API service

2. Experiment with Different Delays

   - Locate the `SlowOperationDelay` flag in `flagd.json`
   - Change the delay to different values:
     - `0` (no delay)
     - `500` (fast)
     - `1000` (default)
     - `2000` (slow)
     - `5000` (very slow)
   - Add a new integer flag for `SlowOperationDelay` in `flagd.json`. For example: `10000` (time to grab a coffee). Tip: This might break the application, so you can have a look into the aspire dashboard to see the error logs
   - Test each configuration and observe the impact

3. Monitor Performance Impact

   - Compare response times with different delay values
   - Note how this affects user experience

4. Implement a Dynamic Configuration

   - Consider how you might change this value without redeploying
   - Think about gradual rollout scenarios (e.g., 90% of users get faster performance)
   - Look into <https://flagd.dev/playground/> for the fractional rollout feature example

### Expected Outcome

You'll understand how integer flags can control performance characteristics and see real-time impact on application behavior.

### Learning Outcomes

- Understand that feature flags can be used to control performance characteristics in real-time
- Understand that feature flags can have multiple types, including integers, strings, and booleans

---

## Exercise 5: Data Source Switching

**Goal**: Toggle between different data sources using feature flags

### What You'll Learn

- Architectural impact of feature flags
- Safe data migration techniques
- Boolean flags for major system changes

### Tasks

1. Understand the Data Sources

   - Examine the `EnableDatabaseWinners` flag
   - Look at `WinnersService.cs` to see how it switches between:
     - Database source (SQLite)
     - JSON file source (`winners.json`)

2. Test Data Source Switching

   - Start with `EnableDatabaseWinners = false` (JSON source)
   - Remove the 2024 winner from the JSON file
   - Change to `EnableDatabaseWinners = true` (database source)
   - Compare the data between sources

3. Understand the Migration Pattern

   - Review how the service gracefully handles the switch
   - Consider error handling scenarios
   - Think about data consistency during transitions

### Expected Outcome

You'll see how feature flags can safely control major architectural decisions and enable smooth data migrations.

### Learning Outcomes

- Understand that feature flags can be used to control major architectural decisions, such as data sources
- Understand that feature flags can be used in combination to create complex feature configurations

---

## Exercise 6: A/B Testing Implementation

**Goal**: Implement A/B testing using feature flags

### What You'll Learn

- How to create and manage A/B tests
- Using variant-based feature flags

### Tasks

1. Use the flag `EnableStatsHeader`

   - Using the `EnableStatsHeader` flag, implement a simple A/B test
   - Use the `EvaluationContext` to differentiate between users, making sure to use the `_userId` property
   - Use the "Change User ID" button on the homepage to simulate different users by changing your user ID
   - Observe how the tabs are displayed in the web application

2. Create a new configuration in flagd for `EnableTabs`

   - Add a new flag in `flagd.json` for `EnableTabs`
   - Set it to `true` for some users and `false` for others
   - Don't forget to refresh the browser to see the changes

### Expected Outcome

You will have implemented a basic A/B test using feature flags, allowing you to control which users see different variants of the application.

### Learning Outcomes

- Understand that feature flags can be used for A/B testing, allowing you to experiment with different user experiences
- Understand that feature flags can be used to control the behavior of the application based on user attributes

---

## Exercise 7: Flag Analytics and Monitoring using hooks

**Goal**: Integrate analytics and monitoring for feature flags

### What You'll Learn

- Add telemetry for flag evaluations
- Visualize flag usage in the Aspire dashboard
- Visualize trace data for flag evaluations

### Tasks

1. Add Telemetry for Feature Flags

   - Open `src/Garage.ServiceDefaults/Extensions.cs`
   - Add telemetry hooks to log flag evaluations (`TraceEnricherHook` and `MetricsHook`)
   - Ensure the OTEL is configured to capture these events

2. Visualize Flag Usage

   - Open the Aspire dashboard
   - Visit the `Traces` section
   - Look for traces related to feature flag evaluations
   - Check the `Metrics` section for flag usage statistics

#### Expected Outcome

You will have integrated telemetry for feature flags, allowing you to monitor their usage and performance in the Aspire dashboard.

### Learning Outcomes

- Understand that feature flags can be monitored and analyzed using hooks
- Understand that feature flags can be visualized in any OTEL compatible dashboard using telemetry data

---

## Extra Exercises: Advanced Targeting

- Implement user segmentation based on attributes
- Create time-based flag activation
- Build geographic targeting rules

## Extra Exercises: Custom Hooks

- Implement custom hooks for feature flag evaluations
- Use hooks to log flag evaluation behavior (see <https://openfeature.dev/docs/reference/concepts/hooks/>)

## Extra Exercises: Event Handlers

- Add an event handler to listen to flag changes (see <https://openfeature.dev/specification/sections/events>)

---

## Troubleshooting

### Common Issues

**Application won't start**

- Ensure .NET 9.0 SDK is installed
- Check that all NuGet packages are restored
- Verify docker is running (if using external Redis)

**Feature flags not updating**

- Restart the application after changes
- Check the Aspire dashboard for service status
- Verify flag values in the configuration

**Database errors**

- Ensure SQLite database is created
- Run database migrations if needed
- Check file permissions

### Getting Help

- Review the [OpenFeature Documentation](https://openfeature.dev/)
- Review the [flagd Documentation](https://flagd.dev/)
- Check the [.NET Aspire Documentation](https://learn.microsoft.com/en-us/dotnet/aspire/)
- Ask questions during the workshop session

---

## Conclusion

Congratulations! You've completed the OpenFeature .NET workshop. You should now understand:

✅ Basic feature flag implementation

✅ Performance tuning with feature flags

✅ Architectural decisions using flags

✅ OpenFeature provider integration

✅ Advanced targeting and evaluation

### Next Steps

- Explore other OpenFeature providers (<https://openfeature.dev/ecosystem>)
- Implement feature flags in your own applications
- Join the OpenFeature community
- Share your experience and learnings
