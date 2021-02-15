﻿using System;
using System.Threading.Tasks;
using Covid19Radar.Services.Logs;
using Covid19Radar.ViewModels;
using Moq;
using Prism.Navigation;
using Xunit;

namespace Covid19Radar.UnitTests.ViewModels
{
    public class SendLogCompletePageViewModelTests
    {
        private readonly MockRepository mockRepository;
        private readonly Mock<INavigationService> mockNavigationService;
        private readonly Mock<ILoggerService> mockLoggerService;

        public SendLogCompletePageViewModelTests()
        {
            mockRepository = new MockRepository(MockBehavior.Default);
            mockNavigationService = mockRepository.Create<INavigationService>();
            mockLoggerService = mockRepository.Create<ILoggerService>();
        }

        private SendLogCompletePageViewModel CreateViewModel()
        {
            var vm = new SendLogCompletePageViewModel(
                mockLoggerService.Object,
                mockNavigationService.Object);
            return vm;
        }

        [Fact]
        public void OnClickSendMailCommandTests_Success()
        {
            var testLogId = "test-log-id";

            var unitUnderTest = CreateViewModel();
            unitUnderTest.Initialize(new NavigationParameters { { "logId", testLogId } });

            var actualCalls = 0;
            string actualSubject = default;
            string actualBody = default;
            string[] actualTo = default;
            unitUnderTest.ComposeEmailAsync = (msg) =>
            {
                actualCalls++;
                actualSubject = msg.Subject;
                actualBody = msg.Body;
                actualTo = msg.To.ToArray();
                return Task.CompletedTask;
            };
            unitUnderTest.OnClickSendMailCommand.Execute(null);

            Assert.Equal(1, actualCalls);
            Assert.NotEmpty(actualSubject);
            Assert.Contains(testLogId, actualBody);
            Assert.Single(actualTo);
            Assert.NotEmpty(actualTo[0]);
        }

        [Fact]
        public void OnClickSendMailCommandTests_Exception()
        {
            var testLogId = "test-log-id";

            var unitUnderTest = CreateViewModel();
            unitUnderTest.Initialize(new NavigationParameters { { "logId", testLogId } });

            var actualCalls = 0;
            unitUnderTest.ComposeEmailAsync = (msg) =>
            {
                actualCalls++;
                throw new Exception();
            };
            unitUnderTest.OnClickSendMailCommand.Execute(null);

            Assert.Equal(1, actualCalls);
        }

        [Fact]
        public void OnClickHomeCommandTests()
        {
            var testLogId = "test-log-id";

            var unitUnderTest = CreateViewModel();
            unitUnderTest.Initialize(new NavigationParameters { { "logId", testLogId } });

            unitUnderTest.OnClickHomeCommand.Execute(null);

            mockNavigationService.Verify(x => x.NavigateAsync("/MenuPage/NavigationPage/HomePage"), Times.Once());
        }
    }
}
