using Moq;
using Microsoft.EntityFrameworkCore;

namespace TRS_backend_test {
    public static class MoqExtensions
    {
        public static Mock<DbSet<T>> ReturnsDbSet<T>(this Mock<DbSet<T>> mockSet, IEnumerable<T> data) where T : class
        {
            var queryableData = data.AsQueryable();

            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryableData.Provider);
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryableData.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryableData.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => queryableData.GetEnumerator());

            return mockSet;
        }
    }
}