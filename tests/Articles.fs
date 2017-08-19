module Articles

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
                                      BsonElement("slug", BsonValue.Create "inception-movie");
                                      BsonElement("title", BsonValue.Create "Inception Movie");
                                      BsonElement("description",BsonValue.Create "Movie about dreams");
                                      BsonElement("body", BsonValue.Create "");
                                      BsonElement("createdAt", BsonValue.Create (System.DateTime.Now));
                                      BsonElement("updatedAt", BsonValue.Create (System.DateTime.Now));
                                      BsonElement("favorited", BsonValue.Create true);
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
  testList "Articles" [
    testCase "should return default article" <| fun _ -> 
      let article = extractArticleList None 
      Expect.isEmpty article.article.title "did't work son"

    testCase "Should return the actual article" <| fun _ ->
      let article = { defaultArticle with article = { defaultArticle.article with title = "Model3"} }
      let resultArticle = extractArticleList (Some article)
      Expect.equal resultArticle.article.title "Model3" "Didn't get the same article back"

    testCase "Should return the correct article slug from bson doc" <| fun _ ->
      let convertedBdoc = RealWorld.BsonDocConverter.toArticleList (Some ([samepleBdoc ()]))
      let article = convertedBdoc |> Array.head 
      Expect.equal article.article.slug "inception-movie" "Did not transform bson doc correctly"

    testCase "Should extract the document id from the user" <| fun _ -> 
      let userId = RealWorld.BsonDocConverter.toUserId (samepleBdoc ())
      Expect.equal userId (generatedUserId.ToString()) "Didn't extract the id of the document"
  ]

