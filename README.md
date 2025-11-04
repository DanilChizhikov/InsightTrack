# InsightTrack
![](https://img.shields.io/badge/unity-2022.3+-000.svg)

## Description
A universal analytical service that allows you to send events to all connected adapters simultaneously.
It has the function of enabling/disabling event sending, they will accumulate in the internal buffer.

## Table of Contents
- [Getting Started](#Getting-Started)
    - [Install manually (using .unitypackage)](#Install-manually-(using-.unitypackage))
    - [Install via UPM (using Git URL)](#Install-via-UPM-(using-Git-URL))
- [Features](#Features)
- [API Reference](#api-reference)
  - [Core Interfaces](#core-interfaces)
  - [Base Classes](#base-classes)
  - [Usage Example](#usage-example)

#### `IAnalyticsService`
Main service interface for sending analytics events.

**Properties:**
- `bool IsInitialized` - Indicates if the service has been initialized
- `bool IsSendingActive` - Gets or sets whether events should be sent immediately

**Events:**
- `event Action OnInitialized` - Triggered when the service is fully initialized
- `event Action<ExceptionDispatchInfo> OnSendException` - Triggered when an exception occurs while sending events

**Methods:**
- `Task InitializeAsync(CancellationToken cancellationToken)`
    - Initializes the analytics service asynchronously
    - `cancellationToken`: Token to cancel the initialization

- `void SetSendingActive(bool isActive)`
    - Enables or disables sending of analytics events
    - `isActive`: Whether to enable or disable sending events

- `void SendEvent(IAnalyticEvent analyticEvent)`
    - Sends an analytics event
    - `analyticEvent`: The event to send

#### `IAnalyticsAdapter`
Interface for analytics service adapters.

**Properties:**
- `bool IsInitialized` - Indicates if the adapter has been initialized
- `int InitializeOrder` - The order in which this adapter should be initialized (lower numbers first)

**Methods:**
- `Task InitializeAsync(CancellationToken cancellationToken)`
    - Initializes the adapter asynchronously
    - `cancellationToken`: Token to cancel the initialization

- `void SendEvent(IAnalyticEvent value)`
    - Sends an analytics event
    - `value`: The event to send

#### `IAnalyticEvent`
Base interface for all analytics events.

**Properties:**
- `string Name` - The name of the event]
- [License](#License)

## Getting Started
Prerequisites:
- [GIT](https://git-scm.com/downloads)
- [Unity](https://unity.com/releases/editor/archive) 2022.3+

### Install manually (using .unitypackage)
1. Download the .unitypackage from [releases](https://github.com/DanilChizhikov/InsightTrack/releases/) page.
2. Open com.dtech.insight-track.x.x.x.unitypackage

### Install via UPM (using Git URL)
1. Navigate to your project's Packages folder and open the manifest.json file.
2. Add the following line to the dependencies section:
    - ```json
      "com.dtech.insight-track": "https://github.com/DanilChizhikov/InsightTrack.git",
      ```
3. Unity will automatically import the package.

If you want to set a target version, InsightTrack uses the `v*.*.*` release tag so you can specify a version like #v2.0.0.

For example `https://github.com/DanilChizhikov/InsightTrack.git#v1.0.0`.

## Features

- **Modular Design**: Easily add or remove analytics adapters
- **Asynchronous Initialization**: Non-blocking initialization of analytics services
- **Event Buffering**: Events are buffered until the service is ready
- **Exception Handling**: Built-in error handling and reporting
- **Dependency Injection** ready

## API Reference

### Core Interfaces

#### `IAnalyticsService`
Main service interface for sending analytics events.

**Properties:**
- `bool IsInitialized` - Indicates if the service has been initialized
- `bool IsSendingActive` - Gets or sets whether events should be sent immediately

**Events:**
- `event Action OnInitialized` - Triggered when the service is fully initialized
- `event Action<ExceptionDispatchInfo> OnSendException` - Triggered when an exception occurs while sending events

**Methods:**
- `Task InitializeAsync(CancellationToken cancellationToken)`
    - Initializes the analytics service asynchronously
    - `cancellationToken`: Token to cancel the initialization

- `void SetSendingActive(bool isActive)`
    - Enables or disables sending of analytics events
    - `isActive`: Whether to enable or disable sending events

- `void SendEvent(IAnalyticEvent analyticEvent)`
    - Sends an analytics event
    - `analyticEvent`: The event to send

#### `IAnalyticsAdapter`
Interface for analytics service adapters.

**Properties:**
- `bool IsInitialized` - Indicates if the adapter has been initialized
- `int InitializeOrder` - The order in which this adapter should be initialized (lower numbers first)

**Methods:**
- `Task InitializeAsync(CancellationToken cancellationToken)`
    - Initializes the adapter asynchronously
    - `cancellationToken`: Token to cancel the initialization

- `void SendEvent(IAnalyticEvent value)`
    - Sends an analytics event
    - `value`: The event to send

#### `IAnalyticEvent`
Base interface for all analytics events.

**Properties:**
- `string Name` - The name of the event

### Base Classes

#### `AnalyticsAdapter`
Abstract base class for analytics adapters.

**Properties:**
- `bool IsInitialized` - Indicates if the adapter has been initialized
- `virtual int InitializeOrder` - The order in which this adapter should be initialized (default: 0)

**Methods:**
- `Task InitializeAsync(CancellationToken cancellationToken)`
    - Initializes the adapter and processes any buffered events

- `abstract void SendEvent(IAnalyticEvent value)`
    - Sends an analytics event

- `virtual void Dispose()`
    - Cleans up resources used by the adapter

- `protected virtual Task InitializeProcessingAsync(CancellationToken cancellationToken)`
    - Performs any async initialization required by the adapter
    - Returns: A Task that completes when initialization is done

- `protected abstract void SendBufferEvents()`
    - Sends all buffered events

#### `AnalyticsAdapter<TEvent>`
Generic abstract base class for typed analytics adapters.

**Methods:**
- `override void SendEvent(IAnalyticEvent value)`
    - Sends a typed analytics event
    - `value`: The event to send (must be of type TEvent)

- `protected abstract void SendEvent(TEvent value)`
    - Sends a typed analytics event
    - `value`: The event to send

### Concrete Implementations

#### `AnalyticsService`
Default implementation of `IAnalyticsService`.

**Constructor:**
- `AnalyticsService(IEnumerable<IAnalyticsAdapter> adapters)`
    - Creates a new instance of the analytics service
    - `adapters`: Collection of analytics adapters to use

## Usage Example

```csharp
// Create adapters
var adapters = new List
{
    new MyAnalyticsAdapter1(),
    new MyAnalyticsAdapter2()
};

// Create and initialize the service
var analyticsService = new AnalyticsService(adapters);
var cts = new CancellationTokenSource();
await analyticsService.InitializeAsync(cts.Token);

// Enable event sending
analyticsService.SetSendingActive(true);

// Send an event
analyticsService.SendEvent(new MyCustomEvent("player_level_up", 42));
```

## Creating Custom Events

```csharp
public readonly struct MyCustomEvent : IAnalyticEvent
{
    public string Name { get; }
    public int Level { get; }

    public MyCustomEvent(string name, int level)
    {
        Name = name;
        Level = level;
    }
}
```

## Creating Custom Adapters

```csharp
public class MyAnalyticsAdapter : AnalyticsAdapter<MyAnalyticEvent>
{
    protected override Task InitializeProcessingAsync(CancellationToken cancellationToken)
    {
        // Initialize your analytics SDK here
        return Task.CompletedTask;
    }

    protected override void SendEvent(MyAnalyticEvent value)
    {
        // Send the event to your analytics service
        Debug.Log($"Sending event: {value.Name}");
    }
}
```

## License
This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.