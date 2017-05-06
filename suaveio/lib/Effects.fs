namespace RealWorld.Effects

module DB =
  open Microsoft.Extensions.Configuration
  open MongoDB.Driver
  open System.IO
  open RealWorld.Models 
  open MongoDB.Bson
  open MongoDB.Driver.Linq

  // TODO: Convert side effects to return option types

  let getConfigDbConnection = 
    let builder = ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json")
    builder.Build()

  let getDBClient () = 
    let mongoConn : string = getConfigDbConnection.GetValue("ConnectionStrings:DefaultConnection")
    let client = new MongoClient(mongoConn)
    client.GetDatabase(getConfigDbConnection.GetValue("ConnectionStrings:dbname")) 

  let loginUser (request: UserRequest) (dbClient: IMongoDatabase) = 
    let collection = dbClient.GetCollection<UserDetails>("Users")
    
    // TODO: Finish JWT authentication; Below is an example of how to use filters for mongo
    //let passwordFilter = Builders.Filter.Eq((fun doc -> doc.Password), request.Password)
    let usernameFilter = Builders.Filter.Eq((fun doc -> doc.Email), request.Email)
    //let filter = Builders.Filter.And(passwordFilter, usernameFilter)
    // Return collections to avoid leaking nulls into your program from C#.
    collection.Find(usernameFilter).ToList() |> Seq.first
    
    