using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Charger.Tests
{//AAA> Arrange-Act-Assert
    [TestClass]
    public class ChargerTests
    {
        [TestMethod]
        public void Test_ChargeProperties()
        {
            var foo_src = new TestModels.Foo()
            {
                Id = "20A",
                No = 1,
                Bar = new TestModels.Bar
                {
                    Nums = new List<int> { 1, 3, 5, 7 }
                }
            };
            var foo_trgt = new TestModels.Foo();
            Mshwf.Charger.Charger.ChargeFrom(foo_trgt, foo_src);

            Assert.AreEqual(foo_trgt.Id, foo_src.Id);
            Assert.AreEqual(foo_trgt.No, foo_src.No);
            for (int i = 0; i < foo_src.Bar.Nums.Count; i++)
            {
                Assert.AreEqual(foo_trgt.Bar.Nums[i], foo_src.Bar.Nums[i]);
            }
        }
    }
}
