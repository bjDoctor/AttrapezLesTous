# Attrapez Les Tous!!!

This simple app is a .Net Core 5.0 Web Api, exposing two endpoints to retrieve Pokemons's details, with or without a funny description.


# Pre-requisites

To be able to build and run the application, make sure you have .Net 5.0 SDK installed. 

This app can also be run in Docker container, if you wish to do so, make sure Docker is installed.


# Build and Run the Application

## In Visual Studio

Simply build the `AttrapezLesTous` project and press F5. This should start a new browser window that will allow you to explore the API. 

This will also launch a console window where logs are available. A log file `AttrapezLesTous-YYYYMMDD.log` is also created in the binary directory.

## From a terminal

From the solution directory, execute the command: `dotnet build`

You may also run the unit tests with `dotnet test`

The app can be run with: `dotnet .\src\AttrapezLesTousApp\bin\Debug\net5.0\AttrapezLesTousApp.dll`. Then open a new browser and navigate to `https://localhost:5001/pokemon/mewtwo`

## As a container

From the solution directory, to build a docker image, execute the command: `docker build -t attrapez-les-tous -f .\Dockerfile .`

To create a container from that image and immediately start it, execute: `docker run -p 80:5000 attrapez-les-tous`
(This binds the local port 80 to the port 5000 inside the container, where the app is running)

To access the containerized app, open a new browser and navigate to `http://localhost/pokemon/mewtwo`


# Design decisions

## .Net 5.0

I am comfortable with .Net Web APIs, so this was my defacto choice to build this app. I choose .Net 5.0 to give a try to the latest stuff! 

The `WriteAsJsonAsync` method in the custom exception handler (in `Startup.cs`) is .Net 5.0 specific, and it's a very useful feature.

## Solution structure

The solution is composed of two projects:
  - A .Net 5.0 Web Api Application, created from a Visual Studio C# template, with only one controller.
  - A `Core` project which implements the logic of the app in F#. I choose this language because it provides a very concise syntax that allows us to build applications little block by little block (aka function). Also, the JsonProviders from the `FSharp.Data` library is a great and simple way to make web requests.


# Improvements to be production ready

## Caching

The caching mechanism is very basic, it is simply a concurrent dictionary, manually maintained. One issue is that we might have duplicated cached entries: when a Pokemon is queried by name or by Id. Both returned the same data, which will be stored twice in the cache. Moving forward, this would need to be improved, probably by implementing an `IMemoryCache`. 

Also, using Dependency Injection for accessing the cache seems a good approach but how this fits with an F# library (where things are supposed to be immutable and pure), would need to be carefully considered. 

## Config 

At the moment, the Poke API and Fun Translation API urls are hard coded in F# files. These should be provided through an config file.

## Adding a Service layer

Ideally, each endpoint in a controller should be a call to a single function which would handle dispatching work to the appropriate place and deal with exceptions. Having a new "Service layer" to do that is probably the correct approach for production code, but given the small size of this app, it seemed un-necessary
