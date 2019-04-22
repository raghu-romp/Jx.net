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
2. `*jx-for` to generate an array of Jx.net template
3. `*jx-if` to render conditional json rendering 
4. `=>` pipes to convert your source json value to another desired value (think of Angular pipes)


## Examples

Jx.net comes with well-tested examples available in the [GitHub repository](https://github.com/raghu-romp/Jx.net/tree/master/Jx.net.Tests/tests). Below are some examples of transformation. 

| NOTE: For improved readability and simplicity the statements assigning json content are not stringified! |
| --- |


#### 1. Interpolation 
**Source**
```javascript
var sourceJson = {
  "firstName": "John",
  "lastName": "Smith",
  "Age": 40,
  "isAlive": true,
  "phoneNumbers": [
    { "type": "home", "number": "212 555-1234" },
    { "type": "mobile", "number": "123 456-7890" }
  ]
}
```
**Template**
```javascript
var templateJson = 
{
  "FullName": "{{$.firstName}} {{$.lastName}}",
  "Aged": "{{$.Age}}",
  "Mobile": "{{$.phoneNumbers[?(@.type == 'mobile')].number}}"
}
```
**Output**
```javascript
{
  "FullName": "John Smith",
  "Aged": 40,
  "Mobile": "123 456-7890"
}
```

When processing templates, everything inside the moustached placeholders "{{xxx}}" is treated as JsonPath  and the value from the JsonPath is queried and replaced with the matching template placeholders. Hence you are allowed to throw any JsonPath expression inside the moustaches that `Newtonsoft.Json` supports and expect it to be resolved flawlessly. 

Moreover, Jx.net doesn't alter the source data-type when the complete attribute is mapped to an interpolation expression. Notice that the Aged attribute retains it's numeric datatype in the output json. 

Also, if the queried value is a complex datatype such as a JObject or a JArray, the value of the property will be the same. Consider the example below.

**Example Template**
```javascript
var templateJson = 
{
  "FullName": "{{$.firstName}} {{$.lastName}}",
  "Aged": "{{$.Age}}",
  "ContactNumber": "{{$.phoneNumbers[?(@.type == 'mobile')]}}"
}
```
**Output**
```javascript
{
  "FullName": "John Smith",
  "Aged": 40,
  "ContactNumber": { "type": "mobile", "number": "123 456-7890" }
}
```

Notice that because we removed the attribute `.number` from the `ContactNumber` property, its value is resolved as the whole object.


#### 2. *jx-for statement


*jx-for is a block statement. Jx.net block statements are represented as an array with the control command as the first string element followed by one or more templates. In case of *jx-for, only one statement is supported which is the template that needs to be repeated for a source array. Following is the syntax of a *jx-for block.

```json
[
    "*jx-for (<path-to-jarray>) as <alias>",
    {
    	...mandatory-block-1
    }
]
```


| Argument | Description |
| :--- | :--- |
| `path-to-jarray` | JsonPath to the source array from our original source json. |
| `alias` | Alias name to each element in the source array. This can be used in the iteration template's interpolation or subsequent jx-commands as a prefix to the JsonPath in place of `$` (*please see example below to understand more...*)  |


**Example**


<details>
  <summary>Source Json (*click to expand*)</summary>
	
```javascript
{
  "people":
  [
    {
      "firstName": "Heidi",
      "lastName": "Coffey",
      "isAlive": true,
      "age": 67,
      "address": {
        "streetAddress": "Norman Avenue",
        "city": "Avalon",
        "state": "Pennsylvania",
        "postalCode": 7394
      },
      "phoneNumbers": [
        {
          "type": "home",
          "number": "827 406-2872"
        },
        {
          "type": "mobile",
          "number": "902 439-2165"
        }
      ]
    },
    {
      "firstName": "Colon",
      "lastName": "Murphy",
      "isAlive": false,
      "age": 56,
      "address": {
        "streetAddress": "Clifford Place",
        "city": "Greenock",
        "state": "Colorado",
        "postalCode": 9922
      },
      "phoneNumbers": [
        {
          "type": "home",
          "number": "927 599-3033"
        },
        {
          "type": "mobile",
          "number": "862 447-3703"
        }
      ]
    },
    {
      "firstName": "Hodges",
      "lastName": "Giles",
      "isAlive": true,
      "age": 65,
      "address": {
        "streetAddress": "Stuyvesant Avenue",
        "city": "Makena",
        "state": "Washington",
        "postalCode": 8024
      },
      "phoneNumbers": [
        {
          "type": "home",
          "number": "808 486-2959"
        },
        {
          "type": "mobile",
          "number": "899 404-3948"
        }
      ]
    }
  ]
}
```
</details>

**Transformer**

```javascript
[
  "*jx-for($.people) as p",
  {
    "FullName": "{{p.firstName}} {{p.lastName}}",
    "AddressLine1": "#{{p.address.streetAddress}}, {{p.address.city}}",
    "AddressLine2": "{{p.address.state}}, {{p.address.postalCode}}",
    "Mobile": "{{p.phoneNumbers[?(@.type == 'mobile')].number}}"
  }
]
```

<details>
	<summary>Output (*click to expand*)</summary>

```javascript
[
  {
    "FullName": "Heidi Coffey",
    "AddressLine1": "#Norman Avenue, Avalon",
    "AddressLine2": "Pennsylvania, 7394",
    "Mobile": "902 439-2165"
  },
  {
    "FullName": "Colon Murphy",
    "AddressLine1": "#Clifford Place, Greenock",
    "AddressLine2": "Colorado, 9922",
    "Mobile": "862 447-3703"
  },
  {
    "FullName": "Hodges Giles",
    "AddressLine1": "#Stuyvesant Avenue, Makena",
    "AddressLine2": "Washington, 8024",
    "Mobile": "899 404-3948"
  }
]
```
</details>

Did you notice that the interpolation placeholders doesn't look like `JsonPath` anymore? The `JsonPath` typically starts with a `$` sign but in this case it starts with `p` which is the `alias` name to the iteration context of the `*jx-for` statement. This is a powerful feature of Jx.net as this allows you to write your transformation templates nested any any number of levels and still reference any parent context without having to worry about how deep you are in the json tree.

