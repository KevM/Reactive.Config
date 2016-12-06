# Reactive Configuration

[![Build status](https://ci.appveyor.com/api/projects/status/tukcgjiunyh09o3i?svg=true)](https://ci.appveyor.com/project/KevM/reactive-config) 

Reactive Configuration let's you inject type safe configuraion settings into your application's types which can be source one or more Configuration Sources. These sources know how to keep the settings up-to-date using [Observables](http://reactivex.io/documentation/observable.html) under the hood, hence **Reactive Configuration**.    

Currently out of the box we provide:

- [StructureMap](http://structuremap.github.io/) IoC container integration
- JSON configuration source

## Example

Lets take a look at using Reactive Config. To make a configurable type simply add the `IConfigurable` marker interface.

```cs
public class MyConfigured : IConfigured
{
    public bool IsEnabled { get; set; } = true;
    public DateTime EnabledOn { get; set; } = DateTime.UtcNow + TimeSpan.FromDays(-7);
    public string MyAppKey { get; set; } = "sooooouuuuuper seeeecrette app key";
}
```

Next we'll take a dependency on this configured type in one of our application types.

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

We currently don't have a configuration file so we are getting the default properties for the configured type. If you add this JSON file to your current working directory you can override the configuration 

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

cd Reactive.Connfig

./build.cmd
```

The build automation will: pull down any dependencies, build, and run tests.