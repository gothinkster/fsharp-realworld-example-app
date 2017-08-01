namespace RealWorld

  module Models =
    open System.Runtime.Serialization
    open MongoDB.Bson

    [<DataContract>]
    type UserRequestDetails = {
      [<field : DataMember(Name = "username")>]
      username : string;
      [<field : DataMember(Name = "email")>]
      email : string;
      [<field : DataMember(Name = "password")>]
      password : string;
    }

    [<DataContract>]
    type UserRequest = {
      [<field: DataMember(Name = "user")>]
      user : UserRequestDetails;
    }
    
    [<DataContract>]
    type UserDetails = {
      [<field : DataMember(Name = "email")>]
      email : string;
      [<field : DataMember(Name = "token")>]
      token : string;
      [<field : DataMember(Name = "username")>]
      username : string;
      [<field : DataMember(Name = "bio")>]
      bio : string;
      [<field : DataMember(Name = "image")>]
      image : string;
      PasswordHash : string;
    }

    [<DataContract>]
    type User = {
      [<field : DataMember(Name = "user")>]
      user : UserDetails;
      Id : BsonObjectId;
    }

    [<DataContract>]
    type ProfileDetails = {
      [<field : DataMember(Name = "username")>]
      username : string;
      [<field : DataMember(Name = "bio")>]
      bio : string;
      [<field : DataMember(Name = "image")>]
      image : string;
      [<field : DataMember(Name = "following")>]
      following : bool
    }

    [<DataContract>]
    type Profile = {
      [<field : DataMember(Name = "profile")>]
      profile : ProfileDetails;
    }

    // Update the case of these because the datamember attributes don't work
    // when serializing back.
    [<DataContract>]
    type ArticleDetails = {
      [<field : DataMember(Name = "slug")>]
      slug : string;
      [<field : DataMember(Name = "title")>]
      title : string;
      [<field : DataMember(Name = "description")>]
      description : string;
      [<field : DataMember(Name = "body")>]
      body : string;
      [<field : DataMember(Name = "createdat")>]
      createdAt : string;
      [<field : DataMember(Name = "updatedat")>]
      updatedAt : string;
      [<field : DataMember(Name = "favorited")>]
      favorited : bool;
      [<field : DataMember(Name = "favoritesCount")>]
      favoritesCount : uint32;
      [<field : DataMember(Name = "author")>]
      author : ProfileDetails;
      [<field : DataMember(Name = "tagList")>]
      taglist : string array
    }

    [<DataContract>]
    type Article = {
      [<field : DataMember(Name = "article")>]
      article : ArticleDetails;
      Id : BsonObjectId;
    }  

    [<DataContract>]
    type Articles = {
      [<field : DataMember(Name = "articles")>]
      articles : ArticleDetails array;
      [<field : DataMember(Name = "articlesCount")>]
      articlesCount : uint32;
    }

    [<DataContract>]
    type CommentDetails = {
      [<field : DataMember(Name = "id")>]
      id : string;
      [<field : DataMember(Name = "createdAt")>]
      createdAt : string;
      [<field : DataMember(Name = "updatedAt")>]
      updatedAt : string;
      [<field : DataMember(Name = "body")>]
      body : string;
      [<field : DataMember(Name = "author")>]
      author : ProfileDetails;
    }

    [<DataContract>]
    type CommentBody = {
      [<field: DataMember(Name = "body")>]
      body : string;
    }

    [<DataContract>]
    type ArticleComment = {
      [<field : DataMember(Name = "comment")>]
      comment : CommentBody;
    }

    [<DataContract>]
    type Comment = {
      [<field : DataMember(Name = "comment")>]
      comment : CommentDetails;
    }

    [<DataContract>]
    type Comments = {
      [<field : DataMember(Name = "comments")>]
      comments : CommentDetails array;
    }

    // We have to use arrays when serializing. The serializer doesn't understand lists.
    [<DataContract>]
    type TagCloud = {
      [<field : DataMember(Name = "tags")>]
      tags : string array;
    }

    [<DataContract>]
    type ErrorBody = {
      [<field : DataMember(Name = "body")>]
      body : string array;
    }

    [<DataContract>]
    type ErrorReport = {
      [<field : DataMember(Name = "error")>]
      errors : ErrorBody;
    }

    type ArticleOptions = {
      Limit : int;
      Tag : string;
      Author : string;
      Favorited : string;
      Offset : int;
    }