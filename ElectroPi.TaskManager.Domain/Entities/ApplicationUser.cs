using ElectroPi.TaskManager.Domain.Common;
using ElectroPi.TaskManager.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectroPi.TaskManager.Domain.Entities
{

    public sealed class ApplicationUser : AuditableEntity
    {
        public string FullName { get; private set; }

        public string Email { get; private set; }

        public string PasswordHash { get; private set; }

        public UserRole Role { get; private set; }

        public DateTime? LastLoginAt { get; private set; }

        public bool IsActive { get; private set; }

        private readonly List<Project> _projects = [];
        public IReadOnlyCollection<Project> Projects => _projects.AsReadOnly();

        private ApplicationUser() : base()
        {
            FullName = string.Empty;
            Email = string.Empty;
            PasswordHash = string.Empty;
        }

        public static ApplicationUser Create(
            string fullName,
            string email,
            string passwordHash,
            UserRole role = UserRole.Member)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(fullName);
            ArgumentException.ThrowIfNullOrWhiteSpace(email);
            ArgumentException.ThrowIfNullOrWhiteSpace(passwordHash);

            return new ApplicationUser
            {
                FullName = fullName.Trim(),
                Email = email.Trim().ToLowerInvariant(),
                PasswordHash = passwordHash,
                Role = role,
                IsActive = true
            };
        }

        public void RecordLogin()
            => LastLoginAt = DateTime.UtcNow;

        public void Deactivate()
            => IsActive = false;

        public void Activate()
            => IsActive = true;

        public void UpdatePasswordHash(string newPasswordHash)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(newPasswordHash);
            PasswordHash = newPasswordHash;
        }

        public void ChangeRole(UserRole newRole)
            => Role = newRole;
    }
}