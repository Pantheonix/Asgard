namespace Api.Features.Users.GetAll;

public class GetAllUsersRequest
{
    [FromQuery(Name = "username")]
    public string? Username { get; set; }

    [FromQuery(Name = "sortBy")]
    public SortUsersBy? SortBy { get; set; } = SortUsersBy.NameAsc;

    [FromQuery(Name = "page")]
    public int? Page { get; set; } = 1;

    [FromQuery(Name = "pageSize")]
    public int? PageSize { get; set; } = 10;
}

public class GetAllUsersResponse
{
    public IEnumerable<UserDto> Users { get; set; } = new List<UserDto>();
    public int TotalCount { get; set; }
}
