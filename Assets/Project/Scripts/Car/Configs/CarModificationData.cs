using Newtonsoft.Json;
using System;
using UnityEngine;

[Serializable]
[JsonObject(MemberSerialization.OptOut)]
public sealed class CarModificationData
{
    [field: SerializeField]
    [JsonProperty]
    public int CarId { get; set; } = -1;

    [field: SerializeField]
    [JsonProperty]
    public int BodyId { get; set; } = -1;

    [field: SerializeField]
    [JsonProperty]
    public int WheelId { get; set; } = -1;

    [field: SerializeField]
    [JsonProperty]
    public int SpoilerId { get; set; } = -1;
}