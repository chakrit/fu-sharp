
using System.Linq;

using MbUnit.Framework;

namespace Fu.Tests
{
    [TestFixture]
    public class WalkPathTests
    {
        public Step[] CreateDummySteps(int numSteps)
        {
            // clones of fu.Identity so no side effects but also != fu.Identity
            return Enumerable.Repeat(fu.Identity, numSteps)
                .Select(s => (Step)s.Clone())
                .ToArray();
        }

        public IWalkPath CreateWalkPath(Step[] steps)
        {
            return new WalkPath(steps);
        }


        [Test]
        public void DeleteNextTest()
        {
            // create a path with several steps
            var steps = CreateDummySteps(3);
            var path = CreateWalkPath(steps);
            var enumerator = path.GetEnumerator();

            // walk one returnStep
            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreSame(steps[0], enumerator.Current);

            // remove one returnStep
            Assert.AreSame(steps[1], path.DeleteNext());

            // check that next one is last returnStep;
            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreSame(steps[2], enumerator.Current);
        }

        [Test]
        public void StopTest()
        {
            var steps = CreateDummySteps(3);
            var path = CreateWalkPath(steps);

            var enumerator = path.GetEnumerator();

            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreSame(steps[0], enumerator.Current);

            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreSame(steps[1], enumerator.Current);

            path.Stop();
            Assert.IsFalse(enumerator.MoveNext());
        }
    }
}
