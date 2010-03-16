
using System.Linq;

using MbUnit.Framework;

using Moq;

namespace Fu.Tests
{
    // TODO: Dry the test
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

        public IFuContext CreateMockedContext(IWalkPath path)
        {
            var moq = new Mock<IFuContext>();
            moq.SetupGet(c => c.WalkPath).Returns(path);

            return moq.Object;
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

        [Test]
        public void ShouldInsertProperlyEvenIfStepIsComposed()
        {
            var set1 = CreateDummySteps(3);
            var set2 = CreateDummySteps(3);

            var steps = new[] { fu.Compose(set1), fu.Compose(set2) };
            var inserts = CreateDummySteps(3);

            var path = CreateWalkPath(steps);
            var context = CreateMockedContext(path);

            var enumerator = path.GetEnumerator();

            // the first step encountered should be the "composing" step
            // from fu.Compose
            Assert.IsTrue(enumerator.MoveNext());
            Assert.IsNotNull(enumerator.Current);
            Assert.AreNotSame(set1[0], enumerator.Current);

            // invoke the composition step
            enumerator.Current(context);

            // next one should be the step that's composed inside the first set
            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreSame(set1[0], enumerator.Current);

            // if we invoke an Insert now (inside a composed step)
            path.InsertNext(inserts[0]);

            // we should have that as the next step, even if we're inside a composed step
            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreSame(inserts[0], enumerator.Current);

            // fast-forward 2 steps to get out of composed step
            Assert.IsTrue(enumerator.MoveNext());
            Assert.IsTrue(enumerator.MoveNext());

            // try the insert again outside of composed step, it should still works
            path.InsertNext(inserts[1]);

            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreSame(inserts[1], enumerator.Current);

            // re-try inside another composed step, should still works
            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreNotSame(set2[0], enumerator.Current);

            enumerator.Current(context);

            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreSame(set2[0], enumerator.Current);

            path.InsertNext(inserts[1]);

            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreSame(inserts[1], enumerator.Current);

        }
    }
}
