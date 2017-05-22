namespace RealWorld

  module Models =
    open System.Runtime.Serialization
    open MongoDB.Bson

    [<DataContract>]
    type UserRequestDetails = {
      [<field : DataMember(Name = "username")>]
      Username : string;
      [<field : DataMember(Name = "email")>]
      Email : string;
      [<field : DataMember(Name = "password")>]
      Password : string;
    }

    [<DataContract>]
    type UserRequest = {
      [<field: DataMember(Name = "user")>]
      User : UserRequestDetails;
    }
    
    [<DataContract>]
    type UserDetails = {
      [<field : DataMember(Name = "email")>]
      Email : string;
      [<field : DataMember(Name = "token")>]
      Token : string;
      [<field : DataMember(Name = "username")>]
      Username : string;
      [<field : DataMember(Name = "bio")>]
      Bio : string;
      [<field : DataMember(Name = "image")>]
      Image : string;
      PasswordHash : string;
    }

    [<DataContract>]
    type User = {
      [<field : DataMember(Name = "user")>]
      User : UserDetails;
      Id : BsonObjectId
    }

    [<DataContract>]
    type ProfileDetails = {
      [<field : DataMember(Name = "username")>]
      Username : string;
      [<field : DataMember(Name = "bio")>]
      Bio : string;
      [<field : DataMember(Name = "image")>]
      Image : string;
      [<field : DataMember(Name = "following")>]
      Following : bool
    }

    [<DataContract>]
    type Profile = {
      [<field : DataMember(Name = "profile")>]
      Profile : ProfileDetails;
    }

    // Look into if the taglist needs to be included
    [<DataContract>]
    type ArticleDetails = {
      [<field : DataMember(Name = "slug")>]
      Slug : string;
      [<field : DataMember(Name = "title")>]
      Title : string;
      [<field : DataMember(Name = "description")>]
      Description : string;
      [<field : DataMember(Name = "body")>]
      Body : string;
      [<field : DataMember(Name = "createdat")>]
      CreatedAt : string;
      [<field : DataMember(Name = "updatedat")>]
      UpdatedAt : string;
      [<field : DataMember(Name = "favorited")>]
      Favorited : bool;
      [<field : DataMember(Name = "favoritesCount")>]
      FavoritesCount : uint32;
      [<field : DataMember(Name = "author")>]
      Author : ProfileDetails;
      [<field : DataMember(Name = "tagList")>]
      Taglist : string array
    }

    [<DataContract>]
    type Article = {
      [<field : DataMember(Name = "article")>]
      Article : ArticleDetails;
    }

    [<DataContract>]
    type Articles = {
      [<field : DataMember(Name = "articles")>]
      Articles : ArticleDetails array;
      [<field : DataMember(Name = "articlesCount")>]
      ArticlesCount : uint32;
    }

    [<DataContract>]
    type CommentDetails = {
      [<field : DataMember(Name = "id")>]
      Id : uint32;
      [<field : DataMember(Name = "createdAt")>]
      CreatedAt : string;
      [<field : DataMember(Name = "updatedAt")>]
      UpdatedAt : string;
      [<field : DataMember(Name = "body")>]
      Body : string;
      [<field : DataMember(Name = "author")>]
      Author : ProfileDetails;
    }

    [<DataContract>]
    type Comment = {
      [<field : DataMember(Name = "comment")>]
      Comment : CommentDetails;
    }

    [<DataContract>]
    type Comments = {
      [<field : DataMember(Name = "comments")>]
      Comments : CommentDetails array;
    }

    // We have to use arrays when serializing. The serializer doesn't understand lists.
    [<DataContract>]
    type TagCloud = {
      [<field : DataMember(Name = "tags")>]
      Tags : string array;
    }

    [<DataContract>]
    type ErrorBody = {
      [<field : DataMember(Name = "body")>]
      Body : string array;
    }

    [<DataContract>]
    type ErrorReport = {
      [<field : DataMember(Name = "error")>]
      Errors : ErrorBody;
    }

    type ArticleOptions = {
      Limit : int;
      Tag : string;
      Author : string;
      Favorited : string;
      Offset : int;
    }