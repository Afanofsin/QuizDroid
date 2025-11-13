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
    public string Difficulty { get; set; }
    [Column("question_count")]
    public int? Question_Count { get; set; }
    [Column("summary")]
    public string Summary { get; set; }
    [Column("estimated_time")]
    public string Time { get; set; }
}
