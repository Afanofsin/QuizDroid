using System.Collections.Generic;
using Postgrest.Attributes;
using Postgrest.Models;
using UnityEngine;

[Table("questions")]
public class Question : BaseModel
{
    [PrimaryKey("id")]
    public long Id { get; set; }
    
    [Column("quiz_pack_id")]
    public long QuizPackId { get; set; }
    
    [Column("question_text")]
    public string QuestionText { get; set; }
    
    [Column("correct_feedback")]
    public string CorrectFeedback { get; set; }
    
    [Column("incorrect_feedback")]
    public string IncorrectFeedback { get; set; }
    
    [Reference(typeof(Answer))]
    public List<Answer> Answers { get; set; }
}
