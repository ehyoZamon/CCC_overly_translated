using SqliteForUnity3D;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Cynteract.Database
{
    [Serializable]
    public class Feedback { 
        [PrimaryKey, AutoIncrement]
        public int Id
        {
            get; set;
        }
        public int UserID
        {
            get
            {
                return userID;
            }
            set
            {
                userID = value;
            }
        }
        public long Timestamp
        {
            get
            {
                return timestamp;
            }
            set
            {
                timestamp = value;
            }
             
        }
        public int Stars
        {
            get
            {
                return stars;
            }
            set
            {
                stars = value;
            }
        }
        public int Pain
        {
            get
            {
                return pain;
            }
            set
            {
                pain = value;
            }
        }
        public string Comments
        {
            get
            {
                return comments;
            }
            set
            {
                comments = value;
            }
        }
        
        public int userID;
        public long timestamp;
        public int stars, pain;
        public string comments;
        public Feedback()
        {

        }

        public Feedback(long timeStamp, int stars, int pain, string comments)
        {
            this.timestamp = timeStamp;
            this.stars = stars;
            this.pain = pain;
            this.comments = comments;
        }
        public string GetDateString()
        {
            DateTime dateTime = new DateTime(timestamp);
            return dateTime.ToShortDateString()+" "+ dateTime.ToShortTimeString();
        }
    }
}