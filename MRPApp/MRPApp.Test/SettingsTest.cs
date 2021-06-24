using Microsoft.VisualStudio.TestTools.UnitTesting;
using MRPApp.View.Setting;
using System;
using System.Linq;

namespace MRPApp.Test
{
    [TestClass]
    public class SettingsTest
    {
        // Db사이에 중복된 데이터 있는지 테스트
        [TestMethod]
        public void IsDuplicateTest()
        {
            var expectVal = true; // 예상값
            var inputCode = "PC010001"; // DB상에 있는 값

            var code = Logic.DataAccess.GetSettings().Where(d => d.BasicCode.Contains(inputCode)).FirstOrDefault();
            var realVal = code != null ? true : false;

            Assert.AreEqual(expectVal, realVal);
        }

        [TestMethod]
        public void IsCodeSearchTest()
        {
            var expectVal = 2; // 예상값
            var inputCode = "설비"; // DB상에 있는 값

            var code = Logic.DataAccess.GetSettings().Where(d => d.BasicCode.Contains(inputCode)).Count();
            var realVal = code != null ? true : false;

            Assert.AreEqual(expectVal, realVal);
        }
    }
}
