namespace Fedodo.Server.Model.Authentication;

public class User
{
    public Guid Id { get; set; }
    public string? UserName { get; set; }
    public byte[]? PasswordHash { get; set; }
    public byte[]? PasswordSalt { get; set; }
    public string? Role { get; set; }
    public string? PrivateKeyActivityPub { get; set; }
}