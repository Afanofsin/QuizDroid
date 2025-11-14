using Postgrest.Attributes;
using Postgrest.Models;
using UnityEngine;

[Table("quizpacks")]
public class QuizPack : BaseModel
{
    [PrimaryKey("id")]
    public int Id { get; set; }
    [Column("category_id")]
    public int? Category_Id { get; set; }
    [Column("title")]
    public string Title { get; set; }
    [Column("difficulty")]
    public string DifficultyLevel { get; set; }
    [Column("question_count")]
    public int? TotalQuestions { get; set; }
    [Column("summary")]
    public string Summary { get; set; }
    [Column("estimated_time")]
    public string EstimatedTime { get; set; }
    [Column("rating")]
    public int Rating {get; set;}
}
