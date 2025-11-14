using Postgrest.Attributes;
using Postgrest.Models;
using UnityEngine;

[Table("profiles")]
public class Profile : BaseModel
{
    [PrimaryKey("user_id")]
    public string Id { get; set; }
    [Column("username")]
    public string Username { get; set; }
    [Column("xp")]
    public int Xp { get; set; }
    [Column("currency")]
    public int Currency { get; set; }
    [Column("isPremium")]
    public bool IsPremium { get; set; }
    [Column("ads_active")]
    public bool IsAdsActive { get; set;}
}
