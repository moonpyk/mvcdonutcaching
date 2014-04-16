# ASP.NET MVC Extensible Donut Caching #

ASP.NET MVC Extensible Donut Caching brings donut caching to ASP.NET MVC 3 and later. The code allows you to cache all of your page apart from one or more Html.Actions which can be executed every request. Perfect for user specific content.

[![Build status](https://ci.appveyor.com/api/projects/status/snh8n1jjhea9fdot)](https://ci.appveyor.com/project/moonpyk/mvcdonutcaching)

## Download ##

The best way to add donut caching to your MVC project is to use the NuGet package. From within Visual Studio, select *Tools | Library Package Manager* and then choose either Package Manager Console or Manage NuGet Packages. Via the console, just type **install-package MvcDonutCaching** and hit return. From the GUI, just search for **MvcDonutCaching** and click the install button.

## Usage ##

The package adds several overloads to the built-in Html.Action HTML helper. The extra parameter in each overload is named *excludeFromParentCache*. Set this to true for any action that should not be cached, or should have a different cache duration from the rest of the page.

```csharp
@Html.Action("Login", "Account", true)
```

The package also include a DonutOutputCacheAttribute to be used in place of the built-in OutputCacheAttribute. This attribute is typically placed on every controller action that needs be be cached.

You can either specify a fixed duration:

```csharp
[DonutOutputCache(Duration = "300")]
public ActionResult Index()
{
	return View();
}
```

Or, use a cache profile:

```csharp
[DonutOutputCache(CacheProfile = "FiveMins")]
public ActionResult Index()
{
  	return View();
}
```

If you are using cache profiles, be sure to configure the profiles in the web.config. Add the following within the system.web element:

```xml
<caching>
  <outputCacheSettings>
    <outputCacheProfiles>
      <add name="FiveMins" duration="300" varyByParam="*" />
    </outputCacheProfiles>
  </outputCacheSettings>
</caching>
```

You can also configure the output cache to use a custom provider:

```xml
<caching>
  <outputCache defaultProvider="DistributedCacheProvider">
    <providers>
      <add name="DistributedCacheProvider" type="DevTrends.Example.DistributedCacheProvider" />
    </providers>
  </outputCache>
</caching>
```

Note, that a custom provider is not included with this project but you can write one fairly easily by subclassing *System.Web.Caching.OutputCacheProvider*. A number of implementations are also available on the web.

## More Information ##

A comprehensive guide to MVC Extensible Donut Caching is now available on the [DevTrends Blog](http://www.devtrends.co.uk/blog/donut-output-caching-in-asp.net-mvc-3).
