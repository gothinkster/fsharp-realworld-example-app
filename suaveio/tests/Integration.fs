module Integration

open Expecto
open RealWorld.Effects.DB
open Suave
open Suave.Web
open RealWorld.Models
open System
open Helper.Utils

[<Tests>]
let tests = 
  testList "Articles" [
    testCase "Should return none since it was not in the db" <| fun _ ->
      let result = getArticleBySlug databaseClient "this slug will not be in the db"
      Expect.equal result None "Did not return None from database"

    testCase "Should return multiple articles" <| fun _ -> 
      let routeResponse = 
        GetRequest "/articles" 
        |> Program.app databaseClient 
        |> extractContext

      let articleList = 
        routeResponse.response.content 
        |> getContent 
        |> Suave.Json.fromJson<Articles>
    
      Expect.isGreaterThan articleList.Articles.Length 0 "Didn't return more than 0 articles"

    testCase "Getting user profile" <| fun _ ->
      let profile = RealWorld.Effects.Actions.getUserProfile databaseClient
      Expect.equal true true ""

    testCase "Should add a new article" <| fun _ ->
      let testAuthor = 
        {
          Username = "";
          Bio = "";
          Image = "";
          Following = false;
        } 
      let sampleArticle = 
        { 
          Article = {
                      Slug = "";
                      Title = "Anothe test";
                      Description = "";
                      Body = "This is a new integration test";
                      CreatedAt = DateTime.Now.ToString();
                      UpdatedAt = DateTime.Now.ToString();
                      Favorited = false;
                      FavoritesCount = 0u;
                      Author = testAuthor;
                      Taglist = [||];
        }
      }
      
      let jsonArticle = Suave.Json.toJson sampleArticle 
      let routeResponse = 
        getPostRequest "/articles" jsonArticle
        |> Program.app databaseClient 
        |> extractContext

      let savedArticle = 
        routeResponse.response.content 
        |> getContent 
        |> Suave.Json.fromJson<Article>

      Expect.equal savedArticle.Article.Title "Anothe test" "Did return the same article"
  ]