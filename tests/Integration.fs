module Integration

open Expecto
open RealWorld.Effects.DB
open Suave
open Suave.Web
open RealWorld.Models
open System
open Helper.Utils
open MongoDB.Bson
open MongoDB.Bson.Serialization
open MongoDB.Driver.Linq
open MongoDB.Driver
open RealWorld.Models

type SubArticle = {
  Id: string;
  details: string;
}

[<Tests>]
let tests = 
  testList "Integration" [
    testCase "Current working test for trying out impure functions" <| fun _ ->
      
      let printDetails (docFragment : BsonValue) = 
        printfn "Fragment: %A" (docFragment.AsBsonDocument.GetValue("slug"))
        "doc"

      let collection = databaseClient.GetCollection<BsonDocument> "Article"
      let documentList = collection.Find(Builders<BsonDocument>.Filter.Empty)
      let mapToArticle (doc:BsonDocument) = 
        { Id = doc.GetValue("_id").ToString(); details = printDetails (doc.GetElement("article").Value) }
      let serializedArticles = documentList.ToList() 
                                |> List.ofSeq 
                                |> List.map mapToArticle     
      printfn "Document list: %A" (serializedArticles)
      Expect.equal true true String.Empty
  ]