using Volo.Abp.Application.Dtos;

namespace EnkiProblems.Problems;

public class TestDto : EntityDto<int>
{
    public int Score { get; set; }
}
