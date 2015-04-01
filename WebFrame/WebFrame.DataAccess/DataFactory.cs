using System.Data.SqlClient;
using Oracle.DataAccess.Client;
using System.Data;
using WebFrame.DataType.Common.Enums;

namespace WebFrame.DataAccess
{

    public static class DataFactory
    {
        /// <summary>
        /// Varsay�lan olarak Oracle veritaban�na ba�lan�r.
        /// </summary>
        /// <param name="isTransactional">i�lemin transaction ile yap�l�p yap�lmayaca��n� belirtir</param>
        /// <param name="connectionStringName">Ba�lant� c�mlesi</param>
        /// <returns>OracleData class d�nd�r�r</returns>
        public static IData GetDataObject( string connectionStringName)
        {
            return new SqlData(connectionStringName);
            //return new OracleData(connectionStringName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isTransactional"></param>
        /// <param name="connectionStringName"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static IData GetDataObject( string connectionStringName, DataProvider provider)
        {
            switch (provider)
            {
                case DataProvider.Oracle:
                    return new OracleData( connectionStringName);
                case DataProvider.SqlServer:
                    return new SqlData(connectionStringName);  
                default:
                    return new SqlData(connectionStringName);
            }
        }
    } 
}