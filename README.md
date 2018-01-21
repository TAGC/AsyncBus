## AsyncBus

AsyncBus is a fully-asynchronous, in-memory implementation of a message bus used to support usages of the publish-subscribe pattern with an application's architecture.

It is inspired by and similar to MemBus (TODO: insert link to Github repo). The motivation for this was to provide better support for applications relying heavily on the Task Asynchronous Pattern (i.e. async/await), such as in ASP.NET Core projects.

A key distinction between AsyncBus and MemBus is that it allows you to subscribe callbacks that return a task and can therefore run asynchronously. Publishing messages on the bus will return a task that only signals completion when the tasks returned by all notified subscribers run to completion themselves.

### Installation

AsyncBus targets the .NET Standard and can be used within .NET Core and .NET Framework applications. To install this package using the dotnet CLI, run:

```
dotnet add package AsyncBus
```