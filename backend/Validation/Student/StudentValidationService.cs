using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Repositories;
using backend.Settings;
using Microsoft.Extensions.Options;

namespace backend.Validation.Student
{
    public class StudentValidationService : IStudentValidationService
    {
        private readonly StudentSettings _studentSettings;
        private readonly IConfigurationRepository _configurationRepository;
        public StudentValidationService(
            IOptions<StudentSettings> studentSettings,
            IConfigurationRepository configurationRepository)
        {
            _studentSettings = studentSettings.Value;
            _configurationRepository = configurationRepository;
        }
        public bool ValidatePhone(string phoneNumber)
        {
            var regex = new System.Text.RegularExpressions.Regex(_studentSettings.PhoneNumber);
            if (!regex.IsMatch(phoneNumber))
                return false;
            return true;
        }

        public bool ValidateEmail(string email)
        {
            if (!email.EndsWith(_studentSettings.EmailDomain))
                return false;
            return true;
        }

        public async Task ValidateStudentAsync(string phoneNumber, string email)
        {
            var shouldPhoneValidationBeApplied = await _configurationRepository.GetConfigurationByKeyAsync("AllowedPhonePattern");
            if (shouldPhoneValidationBeApplied != null && shouldPhoneValidationBeApplied.IsActive)
            {
                if (ValidatePhone(phoneNumber))
                    throw new Exception("Số điện thoại không hợp lệ");
            }

            var shouldEmailValidationBeApplied = await _configurationRepository.GetConfigurationByKeyAsync("AllowedEmailDomain");
            if (shouldEmailValidationBeApplied != null && shouldEmailValidationBeApplied.IsActive)
            {
                if (ValidateEmail(email))
                    throw new Exception("Email không hợp lệ");
            }
        }
    }
}