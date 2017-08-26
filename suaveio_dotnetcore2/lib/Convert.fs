namespace RealWorld
module Convert =
  open RealWorld.Models
  open MongoDB.Bson
  open MongoDB.Driver

  let userRequestToUser (user: UserRequest) = 
    {
      user = {
                username = user.user.username;
                email = user.user.email;
                passwordhash = "";
                token = "";
                bio = "";
                image = "";                
                favorites=[||];
                following=[||];
            };
      Id = "";
    }

  let userToProfile (user: User) = 
    {
      profile = {
                  username = user.user.username;
                  bio = user.user.bio;
                  image = user.user.image;
                  following = true;
      }
    }
  
  let updateUser (user:UserDetails) (result : UpdateResult option) : string  =
    match result with
    | Some _ ->  user |> Suave.Json.toJson |> System.Text.Encoding.UTF8.GetString
    | None -> { errors = { body = [|"Error updating this user."|] } } |> Suave.Json.toJson |> System.Text.Encoding.UTF8.GetString

  let defaultProfile =
    { username = ""; bio = ""; image = ""; following = false;}

  let defaultArticle =
    { 
      Id= "";
      article = 
      { slug = ""; 
        title = ""; 
        description = ""; 
        body = ""; 
        createdAt = System.DateTime.Now; 
        updatedAt = System.DateTime.Now; 
        favoriteIds = [||];
        favorited = false; 
        favoritesCount = 0u; 
        author = defaultProfile; 
        tagList = [||]  }
    }

  let extractArticleList (result) =
    match result with
    | Some article -> article
    | None -> defaultArticle

  let defaultAuthor = { 
    username = "";
    bio = "";
    image = "";
    following = false;
  }  

  let checkNullAuthor (art: Article) = 
    if obj.ReferenceEquals(art.article.author, null) then 
      { art with article = {
                              art.article with author = defaultAuthor 
                           }
      }
    else
      art

  let checkNullSlug (art: Article) = 
    if obj.ReferenceEquals(art.article.slug, null) then 
      { art with article = {
                              art.article with slug = ""
                           }
      }
    else
      art
  
  let checkNullString field = if obj.ReferenceEquals(field, null) then "" else field

  let checkFavoriteIds (art: Article) = 
    if obj.ReferenceEquals(art.article.favoriteIds, null) then 
      { art with article = {
                              art.article with favoriteIds = [||]
                           }
      }
    else
      art

  let addDefaultSlug (art: Article) = 
    if String.isEmpty art.article.slug then
      let wordSections = art.article.title.Split() 
                         |> Array.map (fun eachWord -> eachWord.ToLower().Trim())
                         |> String.concat "-"
      
      {art with article = { art.article with slug = wordSections}}
    else
      art 