namespace RealWorld.Effects

module DB =
  open Microsoft.Extensions.Configuration
  open MongoDB.Driver
  open System.IO

  let getConfigDbConnection = 
    let builder = ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json")
    builder.Build()

  let getDBClient () = 
    let mongoConn : string = getConfigDbConnection.GetValue("ConnectionStrings:DefaultConnection")
    let client = new MongoClient(mongoConn)
    client.GetDatabase(getConfigDbConnection.GetValue("ConnectionStrings:dbname"))