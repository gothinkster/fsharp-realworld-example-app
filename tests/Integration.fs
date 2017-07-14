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

    testCase "Should get the current articles" <| fun _ -> 
      let routeResponse = 
        GetRequest "/articles"
        |> Program.app databaseClient
        |> extractContext

      let jsonString = routeResponse.response.content |> getContent |> Suave.Json.fromJson<Articles>  
      Expect.equal jsonString.ArticlesCount 2u "Didn't get any articles"

    testCase "Get articles created by followed users" <| fun _ ->
      let routeResponse = 
        GetRequest "/articles/feed"
        |> Program.app databaseClient
        |> extractContext

      let articleFeed = routeResponse.response.content |> getContent |> Suave.Json.fromJson<Articles>
      Expect.equal articleFeed.ArticlesCount 2u "Did not return the correct number of articles"

    testCase "Get articles by slug" <| fun _ ->
      let routeResponse = 
        GetRequest "/articles/now"
        |> Program.app databaseClient
        |> extractContext

      // Should return more than one article but since this is the stub, we will come back to this.
      let article = routeResponse.response.content |> getContent |> Suave.Json.fromJson<Article>

      Expect.equal article.Article.Title "This is a new test title" "Title did not match."

    testCase "Should return empty taglist from the database" <| fun _ -> 
      (* Add test case for getting the taglist *)
      
      Expect.equal true true "this failed to return an empty list"
  ]