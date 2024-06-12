using NUnit.Framework;

namespace Loans.Tests
{
    public class MonthlyRepaymentTestData
    {
        public static IEnumerable<TestCaseData> TestCasesWithReturn
        {
            get
            {
                yield return new TestCaseData(200_000m, 6.5m, 30).Returns(1264.14m);
                yield return new TestCaseData(500_000m, 10m, 30).Returns(4387.86m);
                yield return new TestCaseData(200_000m, 10m, 30).Returns(1755.14m);
            }
        }

        public static IEnumerable<TestCaseData> TestCases
        {
            get
            {
                yield return new TestCaseData(200_000m, 6.5m, 30, 1264.14m);
                yield return new TestCaseData(500_000m, 10m, 30, 4387.86m);
                yield return new TestCaseData(200_000m, 10m, 30, 1755.14m);

            }
        }
    }
}
