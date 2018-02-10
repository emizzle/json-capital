using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace JSONCapital.Tests.Extensions
{
    public static class MockExtensions
    {
        public static void SetupAsync<T>(this Mock<DbSet<T>> mockSet) where T : class
        {
            var emptyList = Enumerable.Empty<T>().AsQueryable();

            mockSet.As<IAsyncEnumerable<T>>()
                .Setup(m => m.GetEnumerator())
                   .Returns(new TestAsyncEnumerator<T>(emptyList.GetEnumerator()));

            mockSet.As<IQueryable<T>>()
                .Setup(m => m.Provider)
                   .Returns(new TestAsyncQueryProvider<T>(emptyList.Provider));

            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(emptyList.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(emptyList.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => emptyList.GetEnumerator());
        }
    }
}
