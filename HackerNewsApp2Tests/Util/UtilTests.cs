using Microsoft.VisualStudio.TestTools.UnitTesting;
using HackerNewsApp2.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackerNewsApp2.Util.Tests
{
    [TestClass()]
    public class UtilTests
    {
        [TestMethod()]
        public void GetTimeAgoTest()
        {
            var s = Util.GetTimeAgo(DateTime.Now);
            Assert.AreEqual("?", s);

            var s2 = Util.GetTimeAgo(DateTime.Now.AddHours(-10));
            Assert.AreEqual("9 hrs", s2);
        }
    }
}