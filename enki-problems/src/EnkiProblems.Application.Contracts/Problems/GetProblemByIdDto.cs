using System;
using System.ComponentModel.DataAnnotations;

namespace EnkiProblems.Problems;

public class GetProblemByIdDto
{
    [Required]
    public Guid ProblemId { get; set; }
}
