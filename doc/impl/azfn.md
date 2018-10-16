# Azure Functions

**Package**: [LogMagic.Microsoft.Azure.Functions](https://www.nuget.org/packages/LogMagic.Microsoft.Azure.Functions/)

Provides integration with [Azure Functions v2](https://docs.microsoft.com/en-gb/azure/azure-functions/) Runtime. Azure functions already provide [integration with Application Insights](https://azure.microsoft.com/en-us/updates/azure-functions-now-integrated-with-application-insights/), however it forced you to depend on specific technology and makes transition to other logging system extremely hard in future.

Moreover, if you are already using LogMagic in your logic, it's extremely nice to integrate seamlessly with Azure Functions without destroying the usual workflow by using the built-in `TraceWriter` or `ILogger` interfaces.

## Installing

After referencing the NuGet package, go to your function class and add the following attribute:

```csharp
[LogMagicInvocationFilter]
public static class Function1
{
   //your code...
}
````

This is all you have to do.

## What does it do

This attribute implements a [function filter](https://github.com/Azure/azure-webjobs-sdk/wiki/Function-Filters) that intercepts incoming requests to the function and logs it by default as a *Request* call. It also sets up the logging session so that further logging calls are correlated to this function call.

You can continue using LogMagic as usual, including [using Application Insights](azure-appinsights.md)