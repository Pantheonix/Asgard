namespace Api.Features.Users.GetAll;

public class GetAllUsersRequest { }

public class GetAllUsersResponse
{
    public IEnumerable<UserDto> Users { get; set; } = new List<UserDto>();
}
