using Loans.Domain.Applications;
using Moq;
using NUnit.Framework;
using Moq.Protected;


namespace Loans.Tests
{
    public class LoanApplicationProcessorShould
    {
        [Test]
        public void DeclineLowSalary()
        {
            LoanProduct product = new LoanProduct(99, "Loan", 5.25m);
            LoanAmount amount = new LoanAmount("USD", 200_000);
            var application = new LoanApplication(42, product, amount, "Sarah", 25, "133 Pluralsight Dr., Draper, Utah", 64_999);

            var mockIdentityVerifier = new Mock<IIdentityVerifier>();
            var mockCreditScorer = new Mock<ICreditScorer>();

            var sut = new LoanApplicationProcessor(mockIdentityVerifier.Object, mockCreditScorer.Object);
            sut.Process(application);

            Assert.That(application.GetIsAccepted(), Is.False);

        }

        delegate void ValidateCallback(string applicantName, int applicantAge, string applicantAddress, ref IdentityVerificationStatus status);

        [Test]
        public void Accept()
        {
            LoanProduct product = new LoanProduct(99, "Loan", 5.25m);
            LoanAmount amount = new LoanAmount("USD", 200_000);
            var application = new LoanApplication(42, product, amount, "Sarah", 25, "133 Pluralsight Dr., Draper, Utah", 65_000);

            var mockIdentityVerifier = new Mock<IIdentityVerifier>(MockBehavior.Strict);

            mockIdentityVerifier.Setup(x => x.Initialize());

            //var mockCreditScorer = new Mock<ICreditScorer> { DefaultValue = DefaultValue.Mock};

            var mockCreditScorer = new Mock<ICreditScorer>();

            //mockIdentityVerifier.Setup(x => x.Validate(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>())).Returns(true);

            mockIdentityVerifier.Setup(x => x.Validate("Sarah", 25, "133 Pluralsight Dr., Draper, Utah")).Returns(true);

            //bool isValidOutValue = true;

            //mockIdentityVerifier.Setup(x => x.Validate("Sarah", 25, "133 Pluralsight Dr., Draper, Utah", out isValidOutValue));

            //mockIdentityVerifier.Setup(x => x.Validate("Sarah", 25, "133 Pluralsight Dr., Draper, Utah", ref It.Ref<IdentityVerificationStatus>.IsAny))
            //                    .Callback(new ValidateCallback(
            //                        (string applicantName, int applicantAge, string applicantAddress, ref IdentityVerificationStatus status) =>
            //                    status = new IdentityVerificationStatus(true)));


            //var mockScoreValue = new Mock<ScoreValue>();
            //mockScoreValue.Setup(x => x.Score).Returns(300);

            //var mockScoreResult = new Mock<ScoreResult>();
            //mockScoreResult.Setup(x=> x.ScoreValue).Returns(mockScoreValue.Object);

            //mockCreditScorer.Setup(x => x.ScoreResult).Returns(mockScoreResult.Object);

            mockCreditScorer.SetupAllProperties();
            mockCreditScorer.Setup(x => x.ScoreResult.ScoreValue.Score).Returns(300);
            //mockCreditScorer.SetupProperty(x => x.Count, 10);
            //mockCreditScorer.SetupProperty(x => x.Count);

            var sut = new LoanApplicationProcessor(mockIdentityVerifier.Object, mockCreditScorer.Object);
            sut.Process(application);

            mockCreditScorer.VerifyGet(x => x.ScoreResult.ScoreValue.Score, Times.Once);
            mockCreditScorer.VerifySet(x => x.Count = It.IsAny<int>(), Times.Once);

            Assert.That(application.GetIsAccepted(), Is.True);
            Assert.That(mockCreditScorer.Object.Count, Is.EqualTo(1));
        }

        [Test]
        public void NullReturnExample()
        {
            var mock = new Mock<INullExample>();
            mock.Setup(x => x.SomeMethod());
            //.Returns<string>(null);
            string mockReturnValue = mock.Object.SomeMethod();

            Assert.That(mockReturnValue, Is.Null);
        }

        [Test]
        public void InitializeIdentityVerifier()
        {
            LoanProduct product = new LoanProduct(99, "Loan", 5.25m);
            LoanAmount amount = new LoanAmount("USD", 200_000);
            var application = new LoanApplication(42, product, amount, "Sarah", 25, "133 Pluralsight Dr., Draper, Utah", 65_000);

            var mockIdentityVerifier = new Mock<IIdentityVerifier>();

            var mockCreditScorer = new Mock<ICreditScorer>();

            mockIdentityVerifier.Setup(x => x.Validate("Sarah", 25, "133 Pluralsight Dr., Draper, Utah")).Returns(true);

            mockCreditScorer.Setup(x => x.ScoreResult.ScoreValue.Score).Returns(300);

            var sut = new LoanApplicationProcessor(mockIdentityVerifier.Object, mockCreditScorer.Object);
            sut.Process(application);

            mockIdentityVerifier.Verify(x => x.Initialize());
            mockIdentityVerifier.Verify(x => x.Validate(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()));

            //mockIdentityVerifier.VerifyNoOtherCalls();

        }

        [Test]
        public void CalculateScore()
        {
            LoanProduct product = new LoanProduct(99, "Loan", 5.25m);
            LoanAmount amount = new LoanAmount("USD", 200_000);
            var application = new LoanApplication(42, product, amount, "Sarah", 25, "133 Pluralsight Dr., Draper, Utah", 65_000);

            var mockIdentityVerifier = new Mock<IIdentityVerifier>();

            var mockCreditScorer = new Mock<ICreditScorer>();

            mockIdentityVerifier.Setup(x => x.Validate("Sarah", 25, "133 Pluralsight Dr., Draper, Utah")).Returns(true);

            mockCreditScorer.Setup(x => x.ScoreResult.ScoreValue.Score).Returns(300);

            var sut = new LoanApplicationProcessor(mockIdentityVerifier.Object, mockCreditScorer.Object);
            sut.Process(application);

            mockCreditScorer.Verify(x => x.CalculateScore("Sarah", "133 Pluralsight Dr., Draper, Utah"), Times.Once);
            //mockCreditScorer.Verify(x => x.CalculateScore(It.IsAny<string>(), It.IsAny<string>()));
        }

        [Test]
        public void DeclineWhenCreditScoreError()
        {
            LoanProduct product = new LoanProduct(99, "Loan", 5.25m);
            LoanAmount amount = new LoanAmount("USD", 200_000);
            var application = new LoanApplication(42, product, amount, "Sarah", 25, "133 Pluralsight Dr., Draper, Utah", 65_000);

            var mockIdentityVerifier = new Mock<IIdentityVerifier>();

            var mockCreditScorer = new Mock<ICreditScorer>();

            mockIdentityVerifier.Setup(x => x.Validate("Sarah", 25, "133 Pluralsight Dr., Draper, Utah")).Returns(true);

            mockCreditScorer.SetupAllProperties();
            mockCreditScorer.Setup(x => x.ScoreResult.ScoreValue.Score).Returns(300);

            mockCreditScorer.Setup(x => x.CalculateScore(It.IsAny<string>(), It.IsAny<string>())).Throws<InvalidOperationException>();
            mockCreditScorer.Setup(x => x.CalculateScore(It.IsAny<string>(), It.IsAny<string>())).Throws(new InvalidOperationException("Test Exception"));

            var sut = new LoanApplicationProcessor(mockIdentityVerifier.Object, mockCreditScorer.Object);
            sut.Process(application);

            Assert.That(application.GetIsAccepted(), Is.False);
            //Assert.That(mockCreditScorer.Object.Count, Is.EqualTo(1));
        }

        interface IIdentityVerifierServiceGatewayProtectedMembers
        {
            DateTime GetCurrentTime();
            bool CallService(string applicantName, int applicantAge, string applicantAddress);
        }

        [Test]
        public void AcceptUsingPartialMock()
        {
            LoanProduct product = new LoanProduct(99, "Loan", 5.25m);
            LoanAmount amount = new LoanAmount("USD", 200_000);
            var application = new LoanApplication(42, product, amount, "Sarah", 25, "133 Pluralsight Dr., Draper, Utah", 65_000);

            var mockIdentityVerifier = new Mock<IdentityVerifierServiceGateway>();

            //mockIdentityVerifier.Protected().Setup<bool>("CallService", "Sarah", 25, "133 Pluralsight Dr., Draper, Utah").Returns(true);

            mockIdentityVerifier.Protected().As<IIdentityVerifierServiceGatewayProtectedMembers>().Setup(x => x.CallService(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>())).Returns(true);

            var expectedTime = new DateTime(2000, 1, 1);

            //mockIdentityVerifier.Setup(x => x.GetCurrentTime()).Returns(expectedTime);
            //mockIdentityVerifier.Protected().Setup<DateTime>("GetCurrentTime").Returns(expectedTime);

            mockIdentityVerifier.Protected().As<IIdentityVerifierServiceGatewayProtectedMembers>().Setup(x=> x.GetCurrentTime()).Returns(expectedTime);

            var mockCreditScorer = new Mock<ICreditScorer>();

            mockCreditScorer.Setup(x => x.ScoreResult.ScoreValue.Score).Returns(300);

            var sut = new LoanApplicationProcessor(mockIdentityVerifier.Object, mockCreditScorer.Object);
            sut.Process(application);

            Assert.That(application.GetIsAccepted(), Is.True);
            //Assert.That(mockIdentityVerifier.Object.LastCheckTime, Is.EqualTo(DateTime.Now).Within(1).Seconds);
            Assert.That(mockIdentityVerifier.Object.LastCheckTime, Is.EqualTo(expectedTime));

        }

        public interface INullExample
        {
            string SomeMethod();
        }

    }
}
