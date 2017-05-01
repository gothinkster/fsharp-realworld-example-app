namespace RealWorld

  module Models =
    open System.Runtime.Serialization

    [<CLIMutable>]
    type UserRequest = {
      Username : string;
      Email : string;
      Password : string;
    }

    [<CLIMutable>]
    type UserDetails = {
      Email : string;
      Token : string;
      Username : string;
      Bio : string;
      Image : string;
    }

    [<CLIMutable>]
    type User = {
      User : UserDetails;
    }

    [<CLIMutable>]
    type ProfileDetails = {
      Username : string;
      Bio : string;
      Image : string;
      Following : bool
    }

    [<CLIMutable>]
    type Profile = {
      Profile : ProfileDetails;
    }

    // Look into if the taglist needs to be included
    [<CLIMutableAttribute>]
    type ArticleDetails = {
      Slug : string;
      Title : string;
      Description : string;
      Body : string;
      CreatedAt : string;
      UpdatedAt : string;
      Favorited : bool;
      FavoritesCount : uint32;
      Author : ProfileDetails;
    }

    [<CLIMutable>]
    type Article = {
      Article : ArticleDetails;
    }

    [<CLIMutable>]
    type Articles = {
      Articles : ArticleDetails list;
      ArticlesCount : uint32;
    }

    [<CLIMutable>]
    type CommentDetails = {
      Id : uint32;
      CreatedAt : string;
      UpdatedAt : string;
      Body : string;
      Author : ProfileDetails;
    }

    [<CLIMutable>]
    type Comment = {
      Comment : CommentDetails;
    }

    [<CLIMutableAttribute>]
    type Comments = {
      Comments : CommentDetails list
    }

    [<CLIMutableAttribute>]
    type Tags = {
      Tags : string list;
    }