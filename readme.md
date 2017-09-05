# ![RealWorld Example App](logo.png)

> ### RealWorld Fsharp (Suave + Mongo) codebase containing real world examples (CRUD, auth, advanced patterns, etc) that adheres to the [RealWorld](https://github.com/gothinkster/realworld-example-apps) spec and API.

# Installation

* Install .NET Core 2.0 Framework from here [.NET Core](https://www.microsoft.com/net/core) and Fsharp per [Fsharp.org](http://www.fsharp.org)
* Download MongoDB from here [MongoDB](https://www.mongodb.com/download-center#community)
* Create the mongo db (this can be done easily with Mongo Compass)
* Clone this repo
* Execute the following command on the command line in the directory that has the web framework that you would like to try: dotnet restore
* Execute: dotnet run - This should start the server and you should be able to use a tool like Postman according to the [RealWorld](https://github.com/gothinkster/realworld-example-apps) spec.

# How it works

> When possible the Suave application are divided up into 2 sectors, pure functions and "effectful" functions. All of the "effectful" functions are contained in the effects namespace. These functions in this namespace are in charge of communicating with the database. 

# Getting started

> Prerequisites: 
* See installation section.

