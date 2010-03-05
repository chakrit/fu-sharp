
using MbUnit.Framework;

namespace Fu.Tests
{
    [TestFixture]
    public class WalkPathTests
    {
        [Test]
        public void DeleteNextTest()
        {
            // create a path with several steps
            Step step1 = c => c;
            Step step2 = c => c;
            Step step3 = c => c;

            var path = new WalkPath(new[] { step1, step2, step3 });
            var enumerator = path.GetEnumerator();

            // walk one returnStep
            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreSame(step1, enumerator.Current);

            // remove one returnStep
            Assert.AreSame(step2, path.DeleteNext());

            // check that next one is last returnStep;
            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreSame(step3, enumerator.Current);
        }
    }
}
