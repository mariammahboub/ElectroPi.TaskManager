using ElectroPi.TaskManager.Domain.Entities;
using ElectroPi.TaskManager.Domain.Enums;
using System;

namespace ElectroPi.TaskManager.Application.Tests.Helpers
{
    public sealed class ApplicationUserBuilder
    {
        private string _fullName = "John Doe";
        private string _email = "john.doe@electropi.com";
        private string _passwordHash = "hashed_password_123";
        private UserRole _role = UserRole.Member;

        public ApplicationUserBuilder WithFullName(string name)
        {
            _fullName = name;
            return this;
        }

        public ApplicationUserBuilder WithEmail(string email)
        {
            _email = email;
            return this;
        }

        public ApplicationUserBuilder WithRole(UserRole role)
        {
            _role = role;
            return this;
        }

        public ApplicationUserBuilder WithPasswordHash(string hash)
        {
            _passwordHash = hash;
            return this;
        }

        public ApplicationUser Build()
        {
            var user = ApplicationUser.Create(_fullName, _email, "placeholder", _role);
            user.UpdatePasswordHash(_passwordHash);
            return user;
        }

        public static ApplicationUser Default() => new ApplicationUserBuilder().Build();

        public static ApplicationUser Admin() => new ApplicationUserBuilder().WithRole(UserRole.Admin).Build();

        public static ApplicationUser CreateWithEmail(string email) =>
            new ApplicationUserBuilder().WithEmail(email).Build();
    }
}