namespace Tests.Core;

public class InjectFixture : IDisposable
{
    public readonly UserManager<ApplicationUser> UserManager;
    public readonly AppDbContext DbContext;

    public InjectFixture()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "FakeDatabase")
            .Options;

        DbContext = new AppDbContext(options);

        var users = new List<ApplicationUser>
        {
            new ApplicationUser
            {
                UserName = "Test",
                Id = Guid.NewGuid().ToString(),
                Email = "test@test.it"
            }
        }.AsQueryable();

        var fakeUserManager = new Mock<FakeUserManager>();

        fakeUserManager.Setup(x => x.Users).Returns(users);

        fakeUserManager
            .Setup(x => x.DeleteAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync(IdentityResult.Success);
        fakeUserManager
            .Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);
        fakeUserManager
            .Setup(x => x.UpdateAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync(IdentityResult.Success);
        fakeUserManager
            .Setup(
                x =>
                    x.ChangeEmailAsync(
                        It.IsAny<ApplicationUser>(),
                        It.IsAny<string>(),
                        It.IsAny<string>()
                    )
            )
            .ReturnsAsync(IdentityResult.Success);

        UserManager = fakeUserManager.Object;
    }

    public void Dispose()
    {
        UserManager?.Dispose();
        DbContext?.Dispose();
    }
}
