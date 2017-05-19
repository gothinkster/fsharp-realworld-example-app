# ![RealWorld Example App](logo.png)

> ### RealWorld Fsharp (Suave/AspMVC + Mongo) codebase containing real world examples (CRUD, auth, advanced patterns, etc) that adheres to the [RealWorld](https://github.com/gothinkster/realworld-example-apps) spec and API.

# Installation

* Install .NET Core Framework from here [.NET Core](https://www.microsoft.com/net/core)
* Download MongoDB from here [MongoDB](https://www.mongodb.com/download-center#community)
* Clone this repo
* Execute the following command on the command line in the directory that has the web framework that you would like to try: dotnet restore
* Execute: dotnet run - This should start the server and you should be able to use a tool like Postman according to the [RealWorld](https://github.com/gothinkster/realworld-example-apps) spec.

# How it works

> The Suave application in divided up into 2 sectors, pure functions and effectsfull functions. All of the effectful functions are contained in the effects namespaces. THe functions in this namespace are in charge are communicating with the database. 

# Getting started

> npm install, npm start, etc.

