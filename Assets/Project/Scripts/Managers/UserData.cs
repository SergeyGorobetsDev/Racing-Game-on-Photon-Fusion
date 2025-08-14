using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[JsonObject(MemberSerialization.OptOut)]
public sealed class UserData
{
    [field: SerializeField]
    public string Version { get; set; } = "1.0";

    [field: SerializeField]
    [JsonProperty]
    public string Username { get; set; }
    [field: SerializeField]
    [JsonProperty]
    public string LobbyName { get; set; }
    [field: SerializeField]
    [JsonProperty]
    public int ActiveCarId { get; set; }
    [field: SerializeField]
    [JsonProperty]
    public List<CarModificationData> CarsModificationData { get; set; }
}
