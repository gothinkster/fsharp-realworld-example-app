module Converter

open Expecto
open RealWorld.Convert
open RealWorld.Models
open MongoDB.Bson
open Newtonsoft.Json

let generatedUserId = BsonObjectId(ObjectId.GenerateNewId())

let samepleBdoc () = 
  let author = BsonDocument([
                              BsonElement("username", BsonValue.Create "");
                              BsonElement("bio", BsonValue.Create "");
                              BsonElement("image", BsonValue.Create "");
                              BsonElement("following", BsonValue.Create false);
                            ])
  let articleDetails = BsonDocument([
                                      BsonElement("slug", BsonValue.Create "avengers-movie");
                                      BsonElement("title", BsonValue.Create "Avengers Infinity War Movie");
                                      BsonElement("description",BsonValue.Create "Movie about marvel heros");
                                      BsonElement("body", BsonValue.Create "");
                                      BsonElement("createdAt", BsonValue.Create (System.DateTime.Now));
                                      BsonElement("updatedAt", BsonValue.Create (System.DateTime.Now));
                                      BsonElement("favorited", BsonValue.Create true);
                                      BsonElement("favoriteIds", BsonValue.Create []);
                                      BsonElement("favoritesCount", BsonValue.Create 0);
                                      BsonElement("author", BsonValue.Create author);
                                      BsonElement("tagList", BsonValue.Create []);
                                    ])
  BsonDocument([ 
                BsonElement("article", BsonValue.Create articleDetails) 
                BsonElement("_id", BsonValue.Create (generatedUserId)) 
               ])

[<Tests>]
let tests = 
  testList "Bson Conversion functions" [
    testCase "Should extract the document id" <| fun _ ->
      let extractedId = RealWorld.BsonDocConverter.toUserId (samepleBdoc())      
      Expect.equal (generatedUserId.ToString()) extractedId "Failed to extract the document id"

    testCase "Should create slug from title" <| fun _ -> 
      let bdoc = RealWorld.Convert.defaultArticle
      let docWithoutSlug = { bdoc with article = {bdoc.article with title = "The Last Dragon"} }

      let result = RealWorld.Convert.addDefaultSlug docWithoutSlug
      Expect.equal "the-last-dragon" result.article.slug "Did not properly convert the slug from title"
  ]               