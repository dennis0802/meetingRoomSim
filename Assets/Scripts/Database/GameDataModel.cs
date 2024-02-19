using System;
using System.Collections.Generic;
using Realms;
using MongoDB.Bson;

// https://www.youtube.com/watch?v=tHt9QF9AWsQ
namespace Database{
    public partial class GameDataModel : IRealmObject
    {
        [MapTo("_id")]
        [PrimaryKey]
        public ObjectId Id { get; set; }

        [MapTo("score")]
        public int Score { get; set; }

        [MapTo("user_id")]
        public string UserId { get; set; }

        [MapTo("x")]
        public float X { get; set; }

        [MapTo("y")]
        public float Y { get; set; }
    }
}