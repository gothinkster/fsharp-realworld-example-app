namespace RealWorld.Stubs

open Suave.Json
open RealWorld.Models
open MongoDB.Bson

(*
  These are just stubs that will be eventually replaced with the real implementation.
  They are here as canned response so front end development can be done if need be.
*)
module Responses =
  let singleProfile = 
    {
      Profile = {
                  Username = "test";
                  Bio = "This is a test bio";
                  Image = "";
                  Following = false;
                }
    } |> Suave.Json.toJson |> System.Text.Encoding.UTF8.GetString

  let getFakeAuthor = 
    {
      Username = "SampleJoe";
      Bio = "New programmer";
      Image = "";
      Following = false;
    }

  let singleArticle = 
    {
      Id=(BsonObjectId(ObjectId.GenerateNewId()));
      Article = {
                  Slug = "";
                  Title = "This is a new test title";
                  Description = "This is an example of a canned description.";
                  Body = "This is the main body that the viewer will be reading";
                  CreatedAt = "";
                  UpdatedAt = "";
                  Favorited = false;
                  FavoritesCount = 2u;
                  Author = getFakeAuthor;
                  Taglist = [|"programming"|]
                }
    } |> Suave.Json.toJson |> System.Text.Encoding.UTF8.GetString

  let tagList = 
    {
      Tags = [|"test"; "functional"|]
    } |> Suave.Json.toJson |> System.Text.Encoding.UTF8.GetString

  let singleComment = 
    {
      Comment = {
                  Id = 3123414u;
                  CreatedAt = "";
                  UpdatedAt = "";
                  Body = "Full body of the comment";
                  Author = getFakeAuthor
                }
    } |> Suave.Json.toJson |> System.Text.Encoding.UTF8.GetString

  let multipleComments = 
    {
      Comments = [|
                  {
                    Id = 8293u;
                    CreatedAt = "";
                    UpdatedAt = "";
                    Body = "this is a fake comment";
                    Author = getFakeAuthor;
                  }
      |]  
    } |> Suave.Json.toJson |> System.Text.Encoding.UTF8.GetString

  let multiArticle title description body = 
    [|
      {
        Slug = "Not sure what this is";
        Title = title;
        Description = description;
        Body = body;
        CreatedAt = "";
        UpdatedAt = "";
        Favorited = false;
        FavoritesCount = 2u;
        Author = getFakeAuthor;
        Taglist = [|"functional"|]
      };
      {
        Slug = "what was said before";
        Title = title + " second";
        Description = description + " second one";
        Body = body + " second part";
        CreatedAt = "";
        UpdatedAt = "";
        Favorited = false;
        FavoritesCount = 2u;
        Author = getFakeAuthor;
        Taglist = [|"Rust"|]
      }
    |]

  let multipleArticles = 
    {
      Articles = multiArticle "test title" "some new description" "some presentation body";
      ArticlesCount = 2u;
    } |> Suave.Json.toJson |> System.Text.Encoding.UTF8.GetString