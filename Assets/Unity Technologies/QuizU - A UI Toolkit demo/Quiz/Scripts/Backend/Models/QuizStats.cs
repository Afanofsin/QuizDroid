using Postgrest.Attributes;
using Postgrest.Models;
using UnityEngine;

[Table("quizstats")]
public class QuizStats : BaseModel
{
    [Column("user_id")]
    public string UserId { get; set; }
    
    [Column("quiz_pack_id")]
    public long QuizPackId { get; set; }
    
    [Column("correct")]
    public int Correct { get; set; }
    
    [Column("incorrect")]
    public int Incorrect { get; set; }
    
    [Column("total")]
    public int Total { get; set; }
}
