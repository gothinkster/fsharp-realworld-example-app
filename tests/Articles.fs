module Articles

open Expecto
open RealWorld.Convert
open RealWorld.Models

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
  ]