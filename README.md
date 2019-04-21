[![Build & Tests](https://ci.appveyor.com/api/projects/status/7a6ncfn79vtl6upu?svg=true)](https://ci.appveyor.com/project/raghu-romp/jx-net)


# Jx.net
A simple, yet powerful json structure transformation library. Jx.net uses json based templates and commands that are easy to understand.

## Getting Started

Jx.net is available as a NuGet package. It can be added to your project using Manage NuGet Packages option in Visual Studio or via the Package Management Console.

```
Install-Package Jx.net
```

### Transforming Json

Using Jx.net is a quick 3-step process. 
1. Deserialize your json into `JToken` and Deserialize your transformer into `JToken`
3. Create an instance of  `JsonTransformer` using `JxFactory.Create()`
4. Invoke transformation by calling Transform and pass-in source and transform-template JTokens

```csharp
var source = JsonConvert.DeserializeObject<JToken>(sourceJson);
var template = JsonConvert.DeserializeObject<JToken>(templateJson);
var jx = JxFactory.Create();
var output = jx.Transform(source, template);
```

### Transformation Templates

Although the above 3-step process is what is actually required, to start writing the template, an understand jx transformation commands is necessary.

Jx.net provides very few, but powerful commands to achieve your json transformation. 
1. Interpolation - [Moustache styled `{{$.placeholder}}`](https://en.wikipedia.org/wiki/Mustache_(template_system))
2. `*jx-if` to render conditional json rendering 
3. `*jx-for` to generate an array of json structure
4. `=>` pipes to convert your source json value to another desired value (think of Angular pipes)


## Examples

Jx.net comes with well-tested examples available in the [GitHub repository](https://github.com/raghu-romp/Jx.net/tree/master/Jx.net.Tests/tests). Some of the basic transformation 

#### 1. Interpolation 

**Source**
```json
{
  "firstName": "John",
  "lastName": "Smith",
  "isAlive": true,
  "phoneNumbers": [
    { "type": "home", "number": "212 555-1234" },
    { "type": "mobile", "number": "123 456-7890" }
  ]
}
```
**Template**
```json
{
  "FullName": "{{$.firstName}} {{$.lastName}}",
  "Mobile": "{{$.phoneNumbers[?(@.type == 'mobile')].number}}"
}
```
