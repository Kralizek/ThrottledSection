using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace Tests
{


    [TestFixture]
    public abstract class TestBase
    {
        protected IFixture Fixture;
        protected DateTimeOffset Now;

        [SetUp]
        public void InitializeBase()
        {
            Fixture = new Fixture();
            CustomizeFixture(Fixture);

            Now = Fixture.Create<DateTimeOffset>();
        }

        protected virtual void CustomizeFixture(IFixture fixture)
        {
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        [OneTimeSetUp]
        protected virtual void SetUpFixture()
        {
        }

        [OneTimeTearDown]
        protected virtual void TearDownFixture()
        {
        }
    }

    public abstract class TestBase<T> : TestBase
    {
        protected abstract T CreateSystemUnderTest();
    }


}
