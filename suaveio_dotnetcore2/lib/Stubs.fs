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
      profile = {
                  username = "test";
                  bio = "This is a test bio";
                  image = "";
                  following = false;
                }
    } |> Suave.Json.toJson |> System.Text.Encoding.UTF8.GetString

  let getFakeAuthor = 
    {
      username = "SampleJoe";
      bio = "New programmer";
      image = "";
      following = false;
    }

  let singleArticle = 
    {
      Id=ObjectId.GenerateNewId();
      article = {
                  slug = "";
                  title = "This is a new test title";
                  description = "This is an example of a canned description.";
                  body = "This is the main body that the viewer will be reading";
                  createdAt = System.DateTime.MinValue;
                  updatedAt = System.DateTime.MinValue;
                  favorited = false;
                  favoritesCount = 2u;
                  author = getFakeAuthor;
                  taglist = [|"programming"|]
                }
    } |> Suave.Json.toJson |> System.Text.Encoding.UTF8.GetString

  let tagList = 
    {
      tags = [|"test"; "functional"|]
    } |> Suave.Json.toJson |> System.Text.Encoding.UTF8.GetString

  let singleComment = 
    {
      comment = {
                  id = "";
                  createdAt = "";
                  updatedAt = "";
                  body = "Full body of the comment";
                  author = getFakeAuthor
                }
    } |> Suave.Json.toJson |> System.Text.Encoding.UTF8.GetString

  let multipleComments = 
    {
      comments = [|
                  {
                    id = "";
                    createdAt = "";
                    updatedAt = "";
                    body = "this is a fake comment";
                    author = getFakeAuthor;
                  }
      |]  
    } |> Suave.Json.toJson |> System.Text.Encoding.UTF8.GetString

  let multiArticle title description body = 
    [|
      {
        slug = "Not sure what this is";
        title = title;
        description = description;
        body = body;
        createdAt = System.DateTime.MinValue;
        updatedAt = System.DateTime.MinValue;
        favorited = false;
        favoritesCount = 2u;
        author = getFakeAuthor;
        taglist = [|"functional"|]
      };
      {
        slug = "what was said before";
        title = title + " second";
        description = description + " second one";
        body = body + " second part";
        createdAt = System.DateTime.MinValue;
        updatedAt = System.DateTime.MinValue;
        favorited = false;
        favoritesCount = 2u;
        author = getFakeAuthor;
        taglist = [|"Rust"|]
      }
    |]

  let multipleArticles = 
    {
      articles = multiArticle "test title" "some new description" "some presentation body";
      articlesCount = 2u;
    } |> Suave.Json.toJson |> System.Text.Encoding.UTF8.GetString