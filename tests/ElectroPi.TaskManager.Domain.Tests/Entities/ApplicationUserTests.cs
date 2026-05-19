using ElectroPi.TaskManager.Application.Tests.Helpers;
using ElectroPi.TaskManager.Domain.Entities;
using ElectroPi.TaskManager.Domain.Enums;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ElectroPi.TaskManager.Domain.Tests.Entities
{

    public sealed class ApplicationUserTests
    {

        [Fact]
        public void Create_WithValidInputs_ShouldReturnActiveUser()
        {
            var user = ApplicationUser.Create("Jane Doe", "jane@electropi.com", "hash123");

            user.FullName.Should().Be("Jane Doe");
            user.Email.Should().Be("jane@electropi.com");
            user.PasswordHash.Should().Be("hash123");
            user.Role.Should().Be(UserRole.Member);
            user.IsActive.Should().BeTrue();
            user.LastLoginAt.Should().BeNull();
        }

        [Fact]
        public void Create_ShouldNormaliseEmailToLowercase()
        {
            var user = ApplicationUser.Create("Jane", "JANE@ElectroPi.COM", "hash");

            user.Email.Should().Be("jane@electropi.com");
        }

        [Fact]
        public void Create_ShouldTrimWhitespaceFromFullName()
        {
            var user = ApplicationUser.Create("  Jane Doe  ", "jane@electropi.com", "hash");

            user.FullName.Should().Be("Jane Doe");
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Create_WithEmptyFullName_ShouldThrowArgumentException(string fullName)
        {
            var act = () => ApplicationUser.Create(fullName, "jane@electropi.com", "hash");

            act.Should().Throw<ArgumentException>();
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Create_WithEmptyEmail_ShouldThrowArgumentException(string email)
        {
            var act = () => ApplicationUser.Create("Jane", email, "hash");

            act.Should().Throw<ArgumentException>();
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Create_WithEmptyPasswordHash_ShouldThrowArgumentException(string hash)
        {
            var act = () => ApplicationUser.Create("Jane", "jane@electropi.com", hash);

            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Create_WithAdminRole_ShouldSetAdminRole()
        {
            var user = ApplicationUser.Create("Admin", "admin@electropi.com", "hash", UserRole.Admin);

            user.Role.Should().Be(UserRole.Admin);
        }


        [Fact]
        public void RecordLogin_ShouldSetLastLoginAtToUtcNow()
        {
            var user = ApplicationUserBuilder.Default();
            var before = DateTime.UtcNow.AddSeconds(-1);

            user.RecordLogin();

            user.LastLoginAt.Should().NotBeNull();
            user.LastLoginAt.Should().BeAfter(before);
            user.LastLoginAt.Should().BeBefore(DateTime.UtcNow.AddSeconds(1));
        }

        [Fact]
        public void RecordLogin_CalledTwice_ShouldUpdateLastLoginAt()
        {
            var user = ApplicationUserBuilder.Default();

            user.RecordLogin();
            var firstLogin = user.LastLoginAt;

            System.Threading.Thread.Sleep(10);
            user.RecordLogin();

            user.LastLoginAt.Should().BeAfter(firstLogin!.Value);
        }


        [Fact]
        public void Deactivate_ShouldSetIsActiveToFalse()
        {
            var user = ApplicationUserBuilder.Default();

            user.Deactivate();

            user.IsActive.Should().BeFalse();
        }

        [Fact]
        public void Activate_OnDeactivatedUser_ShouldSetIsActiveToTrue()
        {
            var user = ApplicationUserBuilder.Default();
            user.Deactivate();

            user.Activate();

            user.IsActive.Should().BeTrue();
        }


        [Fact]
        public void UpdatePasswordHash_WithValidHash_ShouldUpdateHash()
        {
            var user = ApplicationUserBuilder.Default();
            var newHash = "new_hashed_password_xyz";

            user.UpdatePasswordHash(newHash);

            user.PasswordHash.Should().Be(newHash);
        }

        [Fact]
        public void UpdatePasswordHash_WithEmptyHash_ShouldThrowArgumentException()
        {
            var user = ApplicationUserBuilder.Default();

            var act = () => user.UpdatePasswordHash(string.Empty);

            act.Should().Throw<ArgumentException>();
        }


        [Fact]
        public void ChangeRole_ShouldUpdateRole()
        {
            var user = ApplicationUserBuilder.Default();
            user.Role.Should().Be(UserRole.Member);

            user.ChangeRole(UserRole.Admin);

            user.Role.Should().Be(UserRole.Admin);
        }
    }
}