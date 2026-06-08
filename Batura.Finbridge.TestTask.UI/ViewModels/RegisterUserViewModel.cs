namespace Batura.Finbridge.TestTask.UI.ViewModels;

public sealed class RegisterUserViewModel
{
    public string FirstName { get; init; } = string.Empty;

    public string LastName { get; init; } = string.Empty;

    public string MiddleName { get; init; } = string.Empty;

    public DateOnly BirthDate { get; init; }

    public string BirthPlace { get; init; } = string.Empty;
}