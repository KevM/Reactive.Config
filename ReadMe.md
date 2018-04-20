# Reactive Configuration

[![Build status](https://ci.appveyor.com/api/projects/status/tukcgjiunyh09o3i?svg=true)](https://ci.appveyor.com/project/KevM/reactive-config) 

Reactive Configuration let's you inject strongly typed configuration into your application sourced from one or more [Configuration Sources](https://github.com/KevM/Reactive.Config/blob/40dad3ac60efae489c10678697c080c4aef64cf0/src/Reactive.Config/IConfigurationSource.cs#L3-L7). These sources keep your configuration up-to-date using [Observables](http://reactivex.io/documentation/observable.html). If your observed configuration changes the next time you "need" your settings they will be up-to-date, hence **Reactive Configuration**.    

## Philosophy 

The goal of this library is to be your application's core supplier of static and dynamic configuration management. With dynamic configuration, we can build a lot of nice things on top of this:
- The basics like application and environment settings.
- [Feature toggles](https://martinfowler.com/bliki/FeatureToggle.html) 
- [A/B testing](https://en.wikipedia.org/wiki/A/B_testing)
- [Service discovery](http://microservices.io/patterns/server-side-discovery.html)

### Today

Currently out of the box we provide:

- JSON configuration source
- [Enviornment configuration source](https://github.com/KevM/Reactive.Config/blob/63c8fa6fb3c8a6dcdf3a68c68388b1100cf08fcd/src/Reactive.Config/Sources/EnvironmentConfigurationSource.cs#L10-L15)
- [StructureMap](http://structuremap.github.io/) IoC container integration

## Application Settings Example

Lets take a look at using Reactive.Config. To make a _configurable type_ simply add the `IConfigurable` marker interface to the type.

```cs
public class MyConfigured : IConfigured
{
    public bool IsEnabled { get; set; } = true;
    public DateTime EnabledOn { get; set; } = DateTime.UtcNow + TimeSpan.FromDays(-7);
    public string MyAppKey { get; set; } = "sooooouuuuuper seeeecrette app key";
}
```

Next we'll take a constructor dependency on this _configured type_ in one of our application types.

```cs
public class MyService
{
    private readonly MyConfigured _config;

    public MyService(MyConfigured config)
    {
        _config = config;
    }

    public void DoSomething()
    {
        if (!_config.IsEnabled || DateTime.Now <= _config.EnabledOn)
        {
            return;
        }

        //login using my appkey
        Console.WriteLine($"Logging in using AppKey: {_config.MyAppKey}");
    }
}
```

Next, we'll setup our IoC container to use the JSON configuration source.  

In this example we use [StructureMap](http://structuremap.github.io/). Here we scan the current assembly for types needing reactive configuration. 

```cs
var container = new Container(_ =>
{
    _.ReactiveConfig(rc =>
    {
        rc.AddSource<JsonConfigurationSource>();
    });

    _.Scan(s =>
    {
        s.TheCallingAssembly();
        s.Convention<ReactiveConfig>();
    });
});
```

```cs
var service = container.GetInstance<MyService>()
service.DoSomething(); 
```

Run this code in a console app and you see this printed out: `Logging in using AppKey: sooooouuuuuper seeeecrette app key`.

### Changing the configuraion:

We have not added JSON configuration file to our project. We get no errors just a default instance of the configured type. 

Now, let's add this JSON file to your current working directory.

#### MyConfigured.json

```json
{
  "MyAppKey" : "a different app key"
}
```

Run the app again and you see `Logging in using AppKey: a different app key`.

## Building 

Open a command line:

```
git clone https://github.com/KevM/Reactive.Config

cd Reactive.Config

./build.cmd
```

The build automation will pull down any dependencies, build, and run all tests.

## Continuous Integration

[AppVeyor](https://appveyor.com) is kind enough to provide CI for this project. Thanks!

- [Reactive.Config CI](https://ci.appveyor.com/project/KevM/reactive-config)