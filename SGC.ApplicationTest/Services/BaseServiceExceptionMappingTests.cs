using FluentAssertions;
using Moq;
using SGC.Application.Services.Base;
using SGC.Domain.Exceptions;
using SGC.Domain.Interfaces.ILogger;
using Xunit;

namespace SGC.ApplicationTest.Services
{
    public class BaseServiceExceptionMappingTests
    {
        private sealed class TestService : BaseService
        {
            public TestService(ISGCLogger logger) : base(logger) { }

            public Task ExecuteAsync(Func<Task> action) => ExecuteOperacionAsync("TestOperation", action, "test");
        }

        [Fact]
        public async Task ExecuteAsync_WhenArgumentException_ThrowsValidationDomainException()
        {
            var loggerMock = new Mock<ISGCLogger>();
            var sut = new TestService(loggerMock.Object);

            Func<Task> action = () => sut.ExecuteAsync(() => throw new ArgumentException("invalid"));

            var ex = await Assert.ThrowsAsync<ValidationDomainException>(action);
            ex.Message.Should().Be("invalid");

            loggerMock.Verify(l => l.LogError(It.IsAny<string>(), It.IsAny<Exception?>()), Times.Once);
        }

        [Fact]
        public async Task ExecuteAsync_WhenKeyNotFoundException_ThrowsNotFoundDomainException()
        {
            var loggerMock = new Mock<ISGCLogger>();
            var sut = new TestService(loggerMock.Object);

            Func<Task> action = () => sut.ExecuteAsync(() => throw new KeyNotFoundException("missing"));

            var ex = await Assert.ThrowsAsync<NotFoundDomainException>(action);
            ex.Message.Should().Be("missing");

            loggerMock.Verify(l => l.LogError(It.IsAny<string>(), It.IsAny<Exception?>()), Times.Once);
        }

        [Fact]
        public async Task ExecuteAsync_WhenUnexpectedException_ThrowsInfrastructureException_WithSafeMessage()
        {
            var loggerMock = new Mock<ISGCLogger>();
            var sut = new TestService(loggerMock.Object);

            Func<Task> action = () => sut.ExecuteAsync(() => throw new InvalidOperationException("tech detail"));

            var ex = await Assert.ThrowsAsync<InfrastructureException>(action);
            ex.Message.Should().Be("Ocurrió un error interno al procesar la operación.");
            ex.InnerException.Should().NotBeNull();

            loggerMock.Verify(l => l.LogError(It.IsAny<string>(), It.IsAny<Exception?>()), Times.Once);
        }
    }
}
