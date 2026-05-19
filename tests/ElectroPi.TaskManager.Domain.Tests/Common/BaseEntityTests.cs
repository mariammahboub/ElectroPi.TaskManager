using ElectroPi.TaskManager.Application.Tests.Helpers;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ElectroPi.TaskManager.Domain.Tests.Common
{
    public sealed class BaseEntityTests
    {
        [Fact]
        public void NewEntity_ShouldHaveNonEmptyId()
        {
            var user = ApplicationUserBuilder.Default();
            user.Id.Should().NotBeEmpty();
        }

        [Fact]
        public void NewEntity_ShouldSetCreatedAtToUtcNow()
        {
            var before = DateTime.UtcNow.AddSeconds(-1);
            var user = ApplicationUserBuilder.Default();
            var after = DateTime.UtcNow.AddSeconds(1);

            user.CreatedAt.Should().BeAfter(before).And.BeBefore(after);
        }

        [Fact]
        public void TwoEntities_WithSameId_ShouldBeEqual()
        {
            var user = ApplicationUserBuilder.Default();
            var copy = user;

            user.Should().Be(copy);
            (user == copy).Should().BeTrue();
        }

        [Fact]
        public void TwoEntities_WithDifferentIds_ShouldNotBeEqual()
        {
            var user1 = ApplicationUserBuilder.Default();
            var user2 = ApplicationUserBuilder.Default();

            user1.Should().NotBe(user2);
            (user1 != user2).Should().BeTrue();
        }

        [Fact]
        public void GetHashCode_ShouldBeConsistentAcrossCalls()
        {
            var user = ApplicationUserBuilder.Default();

            user.GetHashCode().Should().Be(user.GetHashCode());
        }
    }
}