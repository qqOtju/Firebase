using System;
using Firebase.Firestore;

namespace Data
{
    [Serializable]
    [FirestoreData]
    public struct UserData
    {
        [FirestoreProperty] public int Score { get; set; }
        [FirestoreProperty] public int Power { get; set; }
        [FirestoreProperty] public string UserId { get; set; }
    }
}