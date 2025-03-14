namespace backend.Validation.Student
{
    public interface IStudentValidationService
    {
        bool ValidatePhone(string phoneNumber);
        bool ValidateEmail(string email);
        Task ValidateStudentAsync(string phoneNumber, string email);
    }
}