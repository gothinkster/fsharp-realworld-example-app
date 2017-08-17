namespace RealWorld

  module Models =
    open MongoDB.Bson
    open MongoDB.Bson.Serialization.Attributes

    type UserRequestDetails = {
      username : string;
      email : string;
      password : string;
      hash : string;
    }

    type UserRequest = {
      user : UserRequestDetails;
    }

    type AuthenticatedUserDetails = {
      email: string;
      token: string;
      username: string;
      bio: string;
      image: string;
    }

    type AuthenticatedUser = {
      user : AuthenticatedUserDetails;
    }
    
    type UserDetails = {
      email : string;
      token : string;
      username : string;
      bio : string;
      image : string;
      passwordhash : string;
      favorites : string array;
    }

    type User = {
      user : UserDetails;
      [<BsonId>]
      [<BsonRepresentation(BsonType.ObjectId)>]
      Id : string;
    }

    type ProfileDetails = {
      username : string;
      bio : string;
      image : string;
      following : bool
    }

    type Profile = {
      profile : ProfileDetails;
    }

    // Update the case of these because the datamember attributes don't work
    // when serializing back.
    type ArticleDetails = {
      slug           : string;
      title          : string;
      description    : string;
      body           : string;
      createdAt      : System.DateTime;
      updatedAt      : System.DateTime;
      favorited      : bool;
      favoritesCount : uint32;
      author         : ProfileDetails;
      tagList        : string array
    }

    type Article = {
      article : ArticleDetails;
      [<BsonId>]
      [<BsonRepresentation(BsonType.ObjectId)>]
      Id      : string;
    }  

    type Articles = {
      articles : ArticleDetails array;
      articlesCount : uint32;
    }

    type CommentDetails = {
      id : string;
      createdAt : string;
      updatedAt : string;
      body : string;
      author : ProfileDetails;
    }

    type CommentBody = {
      body : string;
    }

    type ArticleComment = {
      comment : CommentBody;
    }

    type Comment = {
      comment : CommentDetails;
    }

    type Comments = {
      comments : CommentDetails array;
    }

    // We have to use arrays when serializing. The serializer doesn't understand lists.
    type TagCloud = {
      tags : string array;
    }

    type ErrorBody = {
      body : string array;
    }

    type ErrorReport = {
      errors : ErrorBody;
    }

    type ArticleOptions = {
      Limit : int;
      Tag : string;
      Author : string;
      Favorited : string;
      Offset : int;
    }

    [<Literal>]
    let usernameField = "username"