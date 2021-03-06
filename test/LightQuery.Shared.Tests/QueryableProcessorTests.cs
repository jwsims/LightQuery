﻿using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace LightQuery.Shared.Tests
{
    public class QueryableProcessorTests
    {
        private class User
        {
            public string UserName { get; set; }
            public string Email { get; set; }
        }

        private IQueryable<User> GetUsers()
        {
            return new[]
                {
                    new User {UserName = "Joe", Email = "Joe@example.com"},
                    new User {UserName = "Alice", Email = "BettyAlice@example.com"},
                    new User {UserName = "Hank", Email = "Arthur.Hank@example.com"},
                    new User {UserName = "Zack", Email = "Zack@example.com"},
                    new User {UserName = "Dan", Email = "Hammerdan@example.com"}
                }
                .ToList()
                .AsQueryable();
        }

        [Fact]
        public void ArgumentNullExceptionOnNullQueryable()
        {
            var queryOptions = new QueryOptions();
            Assert.Throws<ArgumentNullException>("queryable", () => QueryableProcessor.ApplySorting(null, queryOptions));
        }

        [Fact]
        public void ArgumentNullExceptionOnNullQueryOptions()
        {
            var queryable = new[] {string.Empty}.AsQueryable();
            Assert.Throws<ArgumentNullException>("queryOptions", () => queryable.ApplySorting(null));
        }

        [Fact]
        public void ApplySortByUsernameAscending()
        {
            var users = GetUsers();
            var queryOptions = new QueryOptions
            {
                SortPropertyName = "UserName",
                IsDescending = false
            };
            var actual = users.ApplySorting(queryOptions).Cast<User>();
            Assert.Equal("Alice", actual.First().UserName);
            Assert.Equal("Zack", actual.Last().UserName);
        }

        [Fact]
        public void ApplySortByUsernameDescending()
        {
            var users = GetUsers();
            var queryOptions = new QueryOptions
            {
                SortPropertyName = "UserName",
                IsDescending = true
            };
            var actual = users.ApplySorting(queryOptions).Cast<User>();
            Assert.Equal("Zack", actual.First().UserName);
            Assert.Equal("Alice", actual.Last().UserName);
        }

        [Fact]
        public void ApplySortByEmailAscending()
        {
            var users = GetUsers();
            var queryOptions = new QueryOptions
            {
                SortPropertyName = "Email",
                IsDescending = false
            };
            var actual = users.ApplySorting(queryOptions).Cast<User>();
            Assert.Equal("Arthur.Hank@example.com", actual.First().Email);
            Assert.Equal("Zack@example.com", actual.Last().Email);
        }

        [Fact]
        public void ApplySortByEmailDescending()
        {
            var users = GetUsers();
            var queryOptions = new QueryOptions
            {
                SortPropertyName = "Email",
                IsDescending = true
            };
            var actual = users.ApplySorting(queryOptions).Cast<User>();
            Assert.Equal("Zack@example.com", actual.First().Email);
            Assert.Equal("Arthur.Hank@example.com", actual.Last().Email);
        }

        [Fact]
        public void ApplySortByUsernameAscendingWithCamelCase()
        {
            var users = GetUsers();
            var queryOptions = new QueryOptions
            {
                SortPropertyName = "userName",
                IsDescending = false
            };
            var actual = users.ApplySorting(queryOptions).Cast<User>();
            Assert.Equal("Alice", actual.First().UserName);
            Assert.Equal("Zack", actual.Last().UserName);
        }

        [Fact]
        public void ApplySortByUsernameDescendingWithCamelCase()
        {
            var users = GetUsers();
            var queryOptions = new QueryOptions
            {
                SortPropertyName = "userName",
                IsDescending = true
            };
            var actual = users.ApplySorting(queryOptions).Cast<User>();
            Assert.Equal("Zack", actual.First().UserName);
            Assert.Equal("Alice", actual.Last().UserName);
        }

        [Fact]
        public void ApplySortByEmailAscendingWithCamelCase()
        {
            var users = GetUsers();
            var queryOptions = new QueryOptions
            {
                SortPropertyName = "email",
                IsDescending = false
            };
            var actual = users.ApplySorting(queryOptions).Cast<User>();
            Assert.Equal("Arthur.Hank@example.com", actual.First().Email);
            Assert.Equal("Zack@example.com", actual.Last().Email);
        }

        [Fact]
        public void ApplySortByEmailDescendingWithCamelCase()
        {
            var users = GetUsers();
            var queryOptions = new QueryOptions
            {
                SortPropertyName = "email",
                IsDescending = true
            };
            var actual = users.ApplySorting(queryOptions).Cast<User>();
            Assert.Equal("Zack@example.com", actual.First().Email);
            Assert.Equal("Arthur.Hank@example.com", actual.Last().Email);
        }

        [Fact]
        public void DoNothingByInvalidPropertyName()
        {
            var originalUsers = GetUsers();
            Func<IEnumerable<User>, string> aggregateEmails = users =>
                users
                    .Select(u => u.Email)
                    .Aggregate((current, next) => current + next);
            var originalEmails = aggregateEmails(originalUsers);
            var invalidQueryOptions = new QueryOptions { SortPropertyName = "Age" };
            var sortedUsers = originalUsers.ApplySorting(invalidQueryOptions).Cast<User>();
            var sortedEmails = aggregateEmails(sortedUsers);
            Assert.Equal(originalEmails, sortedEmails);
        }

        [Fact]
        public void DoNothingByNullPropertyName()
        {
            var originalUsers = GetUsers();
            Func<IEnumerable<User>, string> aggregateEmails = users =>
                users
                    .Select(u => u.Email)
                    .Aggregate((current, next) => current + next);
            var originalEmails = aggregateEmails(originalUsers);
            var invalidQueryOptions = new QueryOptions { SortPropertyName = null };
            var sortedUsers = originalUsers.ApplySorting(invalidQueryOptions).Cast<User>();
            var sortedEmails = aggregateEmails(sortedUsers);
            Assert.Equal(originalEmails, sortedEmails);
        }
    }
}
