# Documentation 

Warning: This is currently terrible. It needs improvement.

### Configured Types

Making a configured type is easy. Simply mark it with the `IConfigured` marker interface. 

```cs
public interface IConfigured {}
```

### Creating a Configuration Source

Configured types are created by implementing a `IConfiguredSource`. 

```cs
    public interface IConfigurationSource
    {
        bool Handles<T>() where T : class, IConfigured, new();
        ConfigurationResult<T> Get<T>(ConfigurationResult<T> result) where T : class, IConfigured, new();
    }
```

Configuration stores are responsible for matching the key space for a Configured Type with the source of configuration backing them. Say a source backed by environment variables is asked if it is a source for the POCO whose full qualified namespace is Caldron.Newt.Eye. The source could look for environment variables matching the POCO’s namespace. `“Caldron.Newt.Eye.<propertyname>”` and if if finds them declare it is a source for that POCO.

## Configuration Store Registration 

It will be desirable to have multiple sources of configuration data:

- Type default properties.
- Environment
- Application Settings
- Key Value Stores

## Reactive

Each request for a Configured Type will get the latest config. Under the hood Configuration Sources could be reactive to changes via polling or in the case of [Consul long polling of keys](https://www.consul.io/docs/agent/http.html#blocking-queries) for changes. 

It would be possible to expose an `IObservable<TConfiguredType>` for each Configured Type which would ensure that IoC integrations and long lived types such as those created by Windows Services always have the latest configuration.

The application will declare which stores it is using and the order they will be used to populate the Configured Type. The last store in the chain to source a value for one of the Configured Type’s properties will be the one used. 

## Diagnostics

Getting configuration settings out of a running system is pretty handy when you want to take a snapshot of all the configuration settings values in action. Providing a mechanism to write settings to an intermediate file or some format suitable for importing the settings into a source would be handy for devops tooling.

## Features

- Plain PoCos types opt in with an Interface
  - Settings are partitioned by keys space which usually map to the type via namespace.
  - Alternatively partitioned by a key path attribute or specialized interface.
- IoC integration of configuration sources. Types can simply take a dependency on a concrete config sourced type and get up-to-date config. 
- `IObservable<TConfigtype>` - Alternatively long lived types can use observables to react to configuration changes. Under the hood this is likely how the IoC integration will work and inject updated config types in the container.
- Config is source from many locations. The order of precedence could be configurable.
  - App settings
  - Environment
  - Local config files
  - KV Stores (Consul, Redis)
- Settings are reactive. Changes to the key store(s) should cause new config requests to have refreshed values.
  - If the source file is modified new settings are serialized.
  - KV store. Watch a root key (think consul key index long poll). If the source key value store root key is tickled re-serialize.
- Diagnostics
  - Export current settings to disk and a human friendly format for 
  - Settings sources could be have an export (optional?) mechanism. This would be handy for tooling to save settings as an artifact which could be consumed by the source.
    - Grab every reactive setting type in the system serialize it for each settings source registered. 
    - Where/How to write the settings artifact could be configured via Reactive Settings?

## API
 
- Configured Type - has reactive settings marker interface declaring it as something which expects itself to come from the configuration source
  - Configured types have a key space which is either implicit (namespace) or declared (attribute, specialized interface).
  - Each property will be a keyed by a composite of the key space + the property name (overridable with attribute?)
  - Values will be obtained from Configuration Store.
- Configuration Store - factories for Configured Types.
  - Sources will declare what key spaces they are handling (chain of responsibility).
  - During app start look at all the Configured Types and look in their store for the matching key spaces. For example an environment store would typically use the configured type’s namespace as a key prefix and would look for environment variables present which start with the same prefix. If would declare that it handles a key space.
  - Multiple stores can handle a key space with values materializing from each store with the last store to match a key winning. In the case of reactive stores the last store updated wins.
- Configuration Registry - where configured stores are… configured.
  - Configuration stores registered will be used to source configured types.
  - Their order is important so we need a way to declare the order.

## Technical Challenges

- De-serializing many different config sources.
- Mapping key space and keys to Pocos.
  - Use namespace as key path by default but we’ll need a namespace to key space mapping mechanism (abstracted)
  - Use property names as Keys
  - Support override attributes as people will inevitably want to map legacy Pocos. (Low priority)
- Serializing to a format the configuration source can import.

### Existing Libraries to draw inspiration from

- [FubuCore settings source](https://lostechies.com/chadmyers/2011/06/03/cool-stuff-in-fubucore-no-5-easy-configuration/)
- Service Stack [IAppSettings](https://github.com/ServiceStack/ServiceStack/blob/master/src/ServiceStack.Interfaces/Configuration/IAppSettings.cs)
