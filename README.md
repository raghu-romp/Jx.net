[![Build](https://ci.appveyor.com/api/projects/status/7a6ncfn79vtl6upu?svg=true)](https://ci.appveyor.com/project/raghu-romp/jx-net)


# Jx.net
A simple, yet powerful json structure transformation library. Jx.net uses json based templates and commands that are easy to understand.

## Getting Started

Jx.net is available as a NuGet package. It can be added to your project using Manage NuGet Packages option in Visual Studio or via the Package Management Console.

```
Install-Package Jx.net
```

#### Transforming Json

Using Jx.net is a 3-step process. 
1. Deserialize your json into `JToken` and Deserialize your transformer into `JToken`
3. Create an instance of  `JsonTransformer` using `JxFactory.Create()`
4. Invoke transformation by calling Transform and pass-in source and transform-template JTokens

```csharp
var source = JsonConvert.DeserializeObject<JToken>(sourceJson);
var transformer = JsonConvert.DeserializeObject<JToken>(transformerJson);
var jx = JxFactory.Create();
var output = jx.Transform(source, transformer);
```

