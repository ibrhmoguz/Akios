using System.Runtime.Serialization;
using System;

namespace WebFrame.DataType.Common.ExceptionHandling
{
    [Serializable()]
    public class VeritabaniBaglantiHatasi : ApplicationException
    {

        protected VeritabaniBaglantiHatasi(SerializationInfo xSerializationInfo, StreamingContext xContext)
            : base(xSerializationInfo, xContext)
        {
        }

        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            base.GetObjectData(info, context);
        }

        public VeritabaniBaglantiHatasi(Exception e, string src)
            : base("Veritaban� ba�lant� hatas�!", e)
        {
            base.Source = src;
        }

    }

    [Serializable()]
    public class SQLKomutCalismaHatasi : ApplicationException
    {
        private int errorCode;
        public int ErrorCode
        {
            get 
            {
                return errorCode;
            }
        }

        private string mSQL = "";
        public SQLKomutCalismaHatasi(int errorCode, Exception e, string src, string ssql)
            : base("SQL Komut �al��t�r�lamad�!", e)
        {
            base.Source = src;
        }

        public SQLKomutCalismaHatasi(int errorCode, string msg, Exception e, string src, string ssql)
            : base(msg, e)
        {
            this.errorCode = errorCode;

            base.Source = src;
        }

        protected SQLKomutCalismaHatasi(SerializationInfo xSerializationInfo, StreamingContext xContext)
            : base(xSerializationInfo, xContext)
        {
            mSQL = (string)xSerializationInfo.GetValue("SQL", typeof(string));
        }

        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("SQL", mSQL, typeof(string));
        }
        public string SQL
        {
            get { return mSQL; }
            set { mSQL = value; }
        }

    }
}