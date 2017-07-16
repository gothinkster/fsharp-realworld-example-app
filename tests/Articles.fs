module Articles

open Expecto
open RealWorld.Convert

[<Tests>]
let tests = 
  testList "Articles" [
    testCase "should return default article" <| fun _ -> 
      let article = extractArticleList None 
      Expect.isEmpty article.Article.Title "did't work son"
  ]