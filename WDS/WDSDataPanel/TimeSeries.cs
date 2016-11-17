using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSR.WDS.DataPanel
{
    public enum TsFrequency
    {
        Daily = 1,
        Monthly = 30,
        Quarterly = 90,
        SemiAnnual = 182,
        Annual = 365,
    }

    public struct TimeValue
    {
        DateTime Date;
        double Value;
    }


    public class TimeSeries
    {
        #region Public properties
        public string Identifier { get; private set; }

        // Metadata
        private TsFrequency _Frequency;
        public TsFrequency Frequency {
            get
            {
                if (!MetaDataLoaded)
                {
                    LoadMetaData();
                }
                return _Frequency;
            }
            private set
            {
                _Frequency = value;
            }

        }

        // Currently the maximum frequency is daily, but we are prepared for higher frequencies
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Description { get; set; }
        public double LastValue { get; set; }
        public string FacttableName { get; set; }

        // Selected dimensions. Strings for now. Should probably be dimension members.

        public string Source { get; set; }
        public string Unit { get; set; }
        public string FlowStock { get; set; }
        public string Region { get; set; }
        public string Currency { get; set; }

        public string Comment { get; set; }
        public DateTime UpdateTS { get; set; }
        
        // Data
        public TimeValue[] data;
        #endregion

        #region Private properties and fields
        private bool MetaDataLoaded;
        private bool DataLoaded;

        private OdbcConnection DbConnection;
        #endregion

        #region Methods
        public TimeSeries(OdbcConnection dbCon, string identifier)
        {
            DbConnection = dbCon;
            Identifier = identifier;
        }

        public void LoadMetaData()
        {
            DbConnection.Open();
            var q = DbConnection.CreateCommand();
            q.CommandText 
                = @"select description, comment,
                           frequency, startdate, enddate, lastvalue,
                           source, unit, flow_stock, region, currency,
                           facttablename, updatets
                from mb_timeseriesmetadata_zmq(?)";
            var p = new OdbcParameter();
            p.DbType = System.Data.DbType.String;
            p.Value = Identifier;
            q.Parameters.Add(p);
            var r = q.ExecuteReader();
            while (r.Read())
            {
                Description = r.GetString(0);
                Comment = r.GetString(1);
                var frequencyString = r.GetString(2);
                Frequency = (TsFrequency)Enum.Parse(typeof(TsFrequency), frequencyString, true);
                StartDate = r.GetDateTime(3);
                EndDate = r.GetDate(4);
                LastValue = r.GetDouble(5);
                Source = r.GetString(6);
                Unit = r.GetString(7);
                FlowStock = r.GetString(8);
                var o = r.GetValue(9);
                Region = o == DBNull.Value ? null : (string)o;
                o = r.GetValue(10);
                Currency = o == DBNull.Value ? null : (string)o;
                FacttableName = r.GetString(11);
                UpdateTS = r.GetDateTime(12);

                
            }
            MetaDataLoaded = true;

        }
        #endregion

    }
}
