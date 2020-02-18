using Charger.Tests.TestModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Charger.Tests
{
    [TestClass]
    public class ChargerTests
    {
        [TestMethod]
        public void Test_ChargeFrom()
        {
            var foo_src = new Foo()
            {
                Id = "20A",
                No = 1,
                Bar = new Bar
                {
                    Nums = new List<int> { 1, 3, 5, 7 }
                }
            };
            var foo_trgt = new Foo();
            Mshwf.Charger.Charger.ChargeFrom(foo_trgt, foo_src);

            Assert.AreEqual(foo_trgt.Id, foo_src.Id);
            Assert.AreEqual(foo_trgt.No, foo_src.No);
            for (int i = 0; i < foo_src.Bar.Nums.Count; i++)
            {
                Assert.AreEqual(foo_trgt.Bar.Nums[i], foo_src.Bar.Nums[i]);
            }
        }

        [TestMethod]
        public void Test_ChargeFrom_ReadonlyProps()
        {
            var foo_src = new FooWithReadonlyProps("ST01", new List<Bar> { new Bar { Nums = new List<int> { 2, 3 } }, new Bar { Nums = new List<int> { 9, 8, 7 } } })
            {
                Id = "20A",
                No = 1,
                Bar = new Bar
                {
                    Nums = new List<int> { 1, 3, 5, 7 }
                }
            };

            var foo_trgt = new FooWithReadonlyProps(null, null);
            Mshwf.Charger.Charger.ChargerSettings = new Mshwf.Charger.Settings.ChargerSettings
            { ForceWriteReadonlyProperties = true };
            Mshwf.Charger.Charger.ChargeFrom(foo_trgt, foo_src);

            Assert.AreEqual(foo_trgt.Id, foo_src.Id);
            Assert.AreEqual(foo_trgt.Id, foo_src.Id);
            Assert.AreEqual(foo_trgt.No, foo_src.No);
            Assert.AreEqual(foo_trgt.Get_Id, foo_src.Get_Id);
            for (int i = 0; i < foo_src.Bar.Nums.Count; i++)
            {
                Assert.AreEqual(foo_trgt.Bar.Nums[i], foo_src.Bar.Nums[i]);
            }

            for (int i = 0; i < foo_src.Bars.Count; i++)
            {
                for (int j = 0; j < foo_src.Bars[i].Nums.Count; j++)
                    Assert.AreEqual(foo_trgt.Bars[i].Nums[j], foo_src.Bars[i].Nums[j]);
            }
        }
    }
}
