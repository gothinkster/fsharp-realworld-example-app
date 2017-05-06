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
    let collection = dbClient.GetCollection<UserRequest>("Users")
    let filter = Builders.Filter.Eq((fun doc -> doc.Password), request.Password)

    // Return collections to avoid leaking nulls into your program from C#.
    collection.Find(filter).ToList() |> Seq.first
    
    