using System;
using System.Collections.Generic;
using System.Text;

namespace Charger.Tests.TestModels
{
    public class Foo
    {
        public string Id { get; set; }
        public int No { get; set; }
        public Bar Bar { get; set; }
    }

    public class FooWithReadonlyProps
    {
        public FooWithReadonlyProps(string ro_id, List<Bar> bars)
        {
            Get_Id = ro_id;
            Bars = bars;
        }

        public string Id { get; set; }

        public string Get_Id { get; }
        public int No { get; set; }
        public Bar Bar { get; set; }

        public List<Bar> Bars { get; }
    }
}
