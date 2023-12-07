# InsightTrack
![](https://img.shields.io/badge/unity-2022.3+-000.svg)

## Description
InsightTrack serves as a lightweight system for implementing any analytics into a project and the ability to combine them,
without having to add new event dispatch points to the code for each of them.

## Table of Contents
- [Getting Started](#Getting-Started)
    - [Install manually (using .unitypackage)](#Install-manually-(using-.unitypackage))
    - [Install via UPM (using Git URL)](#Install-via-UPM-(using-Git-URL))
- [Basic Usage](#Basic-Usage)
    - [Initialization](#Initialization)
    - [Custom Adapters & Configs](#Custom-Adapters-&-Configs)
    - [AnalyticsService Methods](#AnalyticsService-Methods)
    - [Use AnalyticsEvent in Inspector](#Use-AnalyticsEvent-in-Inspector)
- [License](#License)

## Getting Started
Prerequisites:
- [GIT](https://git-scm.com/downloads)
- [Unity](https://unity.com/releases/editor/archive) 2022.3+

### Install manually (using .unitypackage)
1. Download the .unitypackage from [releases](https://github.com/DanilChizhikov/InsightTrack/releases/) page.
2. Open InsightTrack.x.x.x.unitypackage

### Install via UPM (using Git URL)
1. Navigate to your project's Packages folder and open the manifest.json file.
2. Add this line below the "dependencies": { line
    - ```json title="Packages/manifest.json"
      "com.danilchizhikov.insight-track": "https://github.com/DanilChizhikov/InsightTrack.git?path=Assets/InsightTrack#0.0.1",
      ```
UPM should now install the package.

## Basic Usage

### Initialization
First, you need to initialize the AnalyticsService, this can be done using different methods.
Here we will show the easiest way, which is not the method that we recommend using!
```csharp
public sealed class AnalyticsServiceBootstrap : MonoBehaviour
{
    [SerializeField] private ExampleConfig _config = default;
    
    private static IAnalyticsService _service;

    public static IAnalyticsService Service => _service;

    private void Awake()
    {
        if (_service != null)
        {
            Destroy(gameObject);
            return;
        }

        var exampleAdapter = new ExampleAdapter(_config);
        _service = new AnalyticsService(exampleAdapter);
        _service.Initialize();
    }
}
```

### Custom Adapters & Configs
In order to create your own custom config for the analytics adapter, 
it is enough to inherit from the abstract AnalyticsConfig class or IAnalyticsConfig interface.
Example Config:
```csharp
public sealed class ExampleConfig : AnalyticsConfig
{
    // Yours config data
}
```

In order to create your own custom analytics adapter, you can use two options, 
inherit from the ready-made abstract class AnalyticsAdapter<TConfig>, which accepts the IAnalyticsConfig as a generic parameter, 
or you can inherit from the IAnalyticsAdapter interface and implement all the methods yourself.
Example Adapter:
```csharp
public sealed class ExampleAdapter : AnalyticsAdapter<ExampleConfig>
{
    public ExampleAdapter(ExampleConfig config) : base(config) { }
    
    protected override void Send(string eventName, string value)
    {
        // some code...
    }
}
```

### AnalyticsService Methods

```csharp
public interface IAnalyticsService : IDisposable
{
    //Need call for initialization system
    void Initialize();
    //Allows you to set a value to a custom custom property
    void SetUserProperty(string propName, string value);
    //Sends an analytical event to those systems that have it, the value in this case will be empty
    void SendEvent(string eventName);
    //Sends an analytical event with a value to those systems that have such a value
    void SendEvent(string eventName, string eventValue);
    //Sends an analytical event with parameters of the Key Value Pair type, while the event value will be empty
    void SendEventParams(string eventName, string paramName, object paramValue);
    //Sends an analytical event with an event value and a data set as KeyValuePairs
    void SendEventParams(string eventName, string eventValue, IDictionary<string, object> parameters);
    //Sends an analytical event with a set of data as KeyValuePairs, while the event value will be empty
    void SendEventParams(string eventName, IDictionary<string, object> parameters);
}
```

### Use AnalyticsEvent in Inspector

Also, analytical events can be selected from the inspector,
which allows you not to use writing event names directly in the code

```csharp
public sealed class Example : MonoBehaviour
{
    [SerializeField, AnalyticsEvent] private string _event = string.Empty;
}
```

## License

MIT