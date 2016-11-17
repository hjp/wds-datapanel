using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WSR.WDS.DataPanel;
using System.Data.Odbc;

namespace WSR.WDS.WDSDataPanelTest
{
    [TestClass]
    public class TimeseriesTests
    {
        [TestMethod]
        public void ConstructorTest()
        {
            var ts = new TimeSeries(null, "gen_pv15igs");
            Assert.AreEqual("gen_pv15igs", ts.Identifier);
        }

        [TestMethod]
        public void LoadMetadataTest()
        {
            var dbcon = new OdbcConnection("DSN=wds");
            var ts = new TimeSeries(dbcon, "gen_pv15igs");
            Assert.AreEqual(TsFrequency.Monthly, ts.Frequency);
            Assert.AreEqual("Stock", ts.FlowStock);

        }
    }
}
