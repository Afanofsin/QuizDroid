using Postgrest.Attributes;
using Postgrest.Models;
using UnityEngine;

[Table("answers")]
public class Answer: BaseModel
{
    [PrimaryKey("id")]
    public long Id { get; set; }
    
    [Column("question_id")]
    public long QuestionId { get; set; }
    
    [Column("answer_text")]
    public string AnswerText { get; set; }
    
    [Column("is_correct")]
    public bool IsCorrect { get; set; }
}
