using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using Oracle.DataAccess.Client;
using IsolationLevel = System.Data.IsolationLevel;

namespace WebFrame.DataAccess
{
    /// <summary>
    /// SqlServer ile ilgili i�lemleri yapmak i�in gerekli metodlar� bar�nd�r�r.
    /// </summary>
    internal class SqlData : IDisposable, IData
    {
        private SqlConnection mConnection;
        private SqlTransaction mTransaction;
        private SqlCommand mCommand;
        private SqlDataAdapter mDataAdapter;
        private static bool mIsTransactional;
        Dictionary<string, object> parameterValues;
        List<SqlParameter> parameters;

        private const IsolationLevel ISOLATION_LEVEL = IsolationLevel.ReadCommitted;

        /// <summary>
        /// ��lemin transaction ile yap�l�p yap�lmad���n� belirtir.
        /// </summary>
        public bool IsTransactional
        {
            get
            {
                return mIsTransactional;
            }
        }

        private string connectionName;
        /// <summary>
        /// Veritaban�na ba�lan�l�rken kullan�lan ba�lant� c�mlesinin ad�. 
        /// </summary>
        public string ConnectionName
        {
            get
            {
                return connectionName;
            }
        }

        private bool disposeEdildi = false;//Dispose methodu ~ taraf�ndan 2. kez �a�r�lmas�n diye kullan�l�yor.
        /// <summary>
        /// SqlServer veritaban�yla i�lem yapmak i�in gerekli metodlar�n bulundu�u s�n�f� olu�turur.
        /// </summary>
        /// <param name="connectionStringName">Veritaban�na ba�lanmak i�in kullan�lacak ba�lant� ad�.</param>
        public SqlData(string connectionStringName)
        {
            if (string.IsNullOrEmpty(connectionStringName))
                throw new ArgumentNullException("connectionStringName", "Ba�lant� ad� bo� olamaz.");

            mConnection = new SqlConnection(ConnectionStringHelper.GetConnectionString(connectionStringName));

            mCommand = new SqlCommand();
            mDataAdapter = new SqlDataAdapter(mCommand);
            mCommand.Connection = mConnection;
            parameterValues = new Dictionary<string, object>();
            parameters = new List<SqlParameter>();
            connectionName = connectionStringName;

        }

        public void Dispose()
        {
            if (!disposeEdildi)
            {
                mConnection.Dispose();
                mCommand.Dispose();
                mDataAdapter.Dispose();

                if (mTransaction != null)
                    mTransaction.Dispose();

                disposeEdildi = true;
            }
        }
        /// <summary>
        /// Veritaban� ba�lant�s�n� a�ar.
        /// </summary>
        private void OpenConnection()
        {
            if (mIsTransactional == false)
            {
                try
                {
                    mConnection.Open();
                }
                //catch (Exception ex)
                //{
                //    ExceptionthrowHelper(ex);
                //}
                finally
                { }
            }
        }
        /// <summary>
        /// Veritaban� ba�lant�s�n� kapat�r.
        /// </summary>
        private void CloseConnection()
        {
            if (mIsTransactional == false)
            {
                try
                {
                    mConnection.Close();
                }
                //catch (Exception ex)
                //{
                //    ExceptionthrowHelper(ex);
                //}
                finally { }

            }

            ClearCommandParameters();
        }
        /// <summary>
        /// Transaction i�lemini ba�lat�r.
        /// </summary>
        public void BeginTransaction()
        {
            try
            {
                if (mIsTransactional != true)
                {
                    mConnection.Open();
                    mTransaction = mConnection.BeginTransaction(ISOLATION_LEVEL);
                    mIsTransactional = true;
                }

            }
            //catch (Exception e)
            //{
            //    ExceptionthrowHelper(e);
            //}

            finally { }

        }
        /// <summary>
        /// Transaction i�lemini bitirir.
        /// </summary>
        public void CommitTransaction()
        {
            if (mIsTransactional)
            {
                mTransaction.Commit();
                mConnection.Close();
                mCommand.Transaction = null;
                mIsTransactional = false;
            }

            ClearCommandParameters();
        }
        /// <summary>
        /// Transaction i�lemini geri al�r.
        /// </summary>
        public void RollbackTransaction()
        {
            if (mIsTransactional)
            {
                mTransaction.Rollback();
                mConnection.Close();
                mCommand.Transaction = null;
                mIsTransactional = false;

            }
            ClearCommandParameters();
        }
        /// <summary>
        /// Geriye tek bir integer de�er d�nd�ren sorgulamalar i�in kullan�l�r. �rne�in
        /// Sum, Count, Avarage gibi.
        /// </summary>
        /// <param name="ssql">�al��t�r�lacak komut.</param>
        /// <returns>��lem sonucu.</returns>
        public int ExecuteStatement(string ssql)
        {
            if (string.IsNullOrEmpty(ssql))
                throw new ArgumentNullException("ssql", "Bo� string de�er verilemez.");

            return ExecuteStatement(ssql, CommandType.Text);
        }
        /// <summary>
        /// Geriye tek bir integer de�er d�nd�ren sorgulamalar i�in kullan�l�r. �rne�in
        /// Sum, Count, Avarage gibi.
        /// </summary>
        /// <param name="ssql">�al��t�r�lacak komut.</param>
        /// <param name="commandType">�al��t�r�lacak olan komut tipi (�rn. Text, Procedure , function)</param>
        /// <returns>��lem sonucu.</returns>
        public int ExecuteStatement(string ssql, CommandType commandType)
        {
            if (string.IsNullOrEmpty(ssql))
                throw new ArgumentNullException("ssql", "Bo� string de�er verilemez.");

            this.OpenConnection();
            int result = 0;

            mCommand.CommandType = commandType;
            mCommand.CommandText = ssql;

            AddParameters(mCommand);

            if (mIsTransactional)
            {
                mCommand.Transaction = mTransaction;
            }
            try
            {
                result = mCommand.ExecuteNonQuery();

            }
            //catch (Exception e)
            //{
            //    ExceptionthrowHelper(e);
            //}
            finally
            {
                this.CloseConnection();
            }
            return result;
        }
        /// <summary>
        /// Dataseti prosed�r kullanarak doldurmak amac�yla kullan�l�r. Bu komut sadece prosed�r ile �al���r.
        /// </summary>
        /// <param name="storedprocedureName">�al��t�r�lacak prosed�r ad�.</param>
        /// <param name="ds">Doldurulacak olan dataset</param>
        public void ExecuteStatement(string storedprocedureName, DataSet ds)
        {
            if (string.IsNullOrEmpty(storedprocedureName))
                throw new ArgumentNullException("storedprocedureName", "Bo� string de�er verilemez.");

            this.OpenConnection();

            try
            {
                mCommand.CommandText = storedprocedureName;
                mCommand.CommandType = CommandType.StoredProcedure;

                AddParameters(mCommand);

                if (mTransaction != null)
                    mCommand.Transaction = mTransaction;

                mDataAdapter.SelectCommand = mCommand;
                mDataAdapter.Fill(ds);
            }
            //catch (Exception ex)
            //{
            //    ExceptionthrowHelper(ex);
            //}
            finally
            {
                this.CloseConnection();
                mCommand.Transaction = null;
            }
        }
        /// <summary>
        /// G�nderilen komut i�erisinde kullan�lan prosed�r veya fonksiyona ait output parametre 
        /// de�erlerini almak i�in kullan�l�r.
        /// </summary>
        /// <param name="storedProcedureOrFunctionName">�al���t�r�lacak prosed�r veya fonksiyon ismi.</param>
        /// <returns>Fonsiyon veya prosed�re ait output parametrelerin ad� ve d�n�� de�erleri</returns>
        public Dictionary<string, object> ExecuteStatementUDI(string storedProcedureOrFunctionName)
        {
            if (string.IsNullOrEmpty(storedProcedureOrFunctionName))
                throw new ArgumentNullException("storedProcedureOrFunctionName", "Bo� string de�er verilemez.");

            parameterValues.Clear();
            this.OpenConnection();
            try
            {
                mCommand.CommandText = storedProcedureOrFunctionName;
                mCommand.CommandType = CommandType.StoredProcedure;
                AddParameters(mCommand);

                if (mTransaction != null)
                    mCommand.Transaction = mTransaction;


                mCommand.ExecuteScalar();

                foreach (SqlParameter op in mCommand.Parameters)
                {
                    if ((op.Direction == ParameterDirection.InputOutput) || (op.Direction == ParameterDirection.Output) || (op.Direction == ParameterDirection.ReturnValue))
                        parameterValues.Add(op.ParameterName, op.Value);
                }

            }
            //catch (Exception ex)
            //{
            //    ExceptionthrowHelper(ex);
            //}
            finally
            {
                this.CloseConnection();
                mCommand.Transaction = null;
            }
            return parameterValues;
        }
        /// <summary>
        /// Geriye  tek bir de�er d�nd�ren sql komutlar�n� �al��t�rmak i�in kullan�l�r.
        /// </summary>
        /// <param name="ssql">�al��t�r�lacak komut.</param>
        /// <param name="komutTipi">�al��t�r�lacak olan komut tipi (�rn. Text, Procedure , function)</param>
        /// <returns>Komutun �al��mas� sonucu elde edilen de�er</returns>
        public object ExecuteScalar(string ssql, CommandType komutTipi)
        {
            if (string.IsNullOrEmpty(ssql))
                throw new ArgumentNullException("ssql", "Bo� string de�er verilemez.");

            this.OpenConnection();
            object result = null;
            mCommand.CommandText = ssql;
            mCommand.CommandType = komutTipi;
            AddParameters(mCommand);

            if (IsTransactional)
            {
                mCommand.Transaction = mTransaction;
            }
            try
            {
                result = mCommand.ExecuteScalar();

            }
            //catch (Exception e)
            //{
            //    ExceptionthrowHelper(e);
            //}
            finally
            {
                this.CloseConnection();
                mCommand.Transaction = null;
            }
            return result;
        }
        /// <summary>
        /// Select ��lemleri i�in kullan�l�r.
        /// </summary>
        /// <param name="ds">Doldurulacak DataSet</param>
        /// <param name="ssql">�al��t�r�lacak komut.</param>
        public void GetRecords(DataSet ds, string ssql)
        {
            if (ds == null)
                throw new ArgumentNullException("ds", "Dataset null olamaz");

            if (string.IsNullOrEmpty(ssql))
                throw new ArgumentNullException("ssql", "Bo� parametre verilemez.");

            if (ds.Tables.Count == 0)
                ds.Tables.Add();
            GetRecords(ds.Tables[0], ssql);
        }
        /// <summary>
        /// Select ��lemleri i�in kullan�l�r.
        /// </summary>
        /// <param name="ds">Doldurulacak DataSet</param>
        /// <param name="ssql">�al��t�r�lacak komut.</param>
        /// <param name="stable">�zerinde �al���lan tablo ad�.</param>
        /// <param name="commandType">�al��t�r�lacak olan komut tipi (�rn. Text, Procedure , function)</param>
        public void GetRecords(DataSet ds, string ssql, string stable, CommandType commandType)
        {
            if (ds == null)
                throw new ArgumentNullException("ds", "Dataset null olamaz");

            if (string.IsNullOrEmpty(ssql))
                throw new ArgumentNullException("ssql", "Bo� parametre verilemez.");

            if (string.IsNullOrEmpty(stable))
                throw new ArgumentNullException("stable", "Bo� parametre verilemez.");

            if (!ds.Tables.Contains(stable))
                ds.Tables.Add(new DataTable(stable));
            GetRecords(ds.Tables[stable], ssql, commandType);
        }
        /// <summary>
        /// Select ��lemleri i�in kullan�l�r.
        /// </summary>
        /// <param name="ds">Doldurulacak DataSet</param>
        /// <param name="ssql">�al��t�r�lacak komut.</param>
        /// <param name="stable">�zerinde �al���lan tablo ad�.</param>
        public void GetRecords(DataSet ds, string ssql, string stable)
        {
            if (ds == null)
                throw new ArgumentNullException("ds", "Dataset null olamaz");

            if (string.IsNullOrEmpty(ssql))
                throw new ArgumentNullException("ssql", "Bo� parametre verilemez.");

            if (string.IsNullOrEmpty(stable))
                throw new ArgumentNullException("stable", "Bo� parametre verilemez.");

            ds.Tables.Add(new DataTable(stable));
            GetRecords(ds.Tables[stable], ssql);
        }
        /// <summary>
        /// Select ��lemleri i�in kullan�l�r.
        /// </summary>
        /// <param name="dt">Doldurulacak DataTable</param>
        /// <param name="ssql">�al��t�r�lacak komut.</param>
        public void GetRecords(DataTable dt, string ssql)
        {
            if (dt == null)
                throw new ArgumentNullException("dt", "Datatable null olamaz");

            if (string.IsNullOrEmpty(ssql))
                throw new ArgumentNullException("ssql", "Bo� parametre verilemez.");

            GetRecords(dt, ssql, CommandType.Text);
        }
        /// <summary>
        /// Select ��lemleri i�in kullan�l�r.
        /// </summary>
        /// <param name="ds">Doldurulacak DataSet</param>
        /// <param name="ssql">�al��t�r�lacak komut.</param>
        /// <param name="commandType">�al��t�r�lacak olan komut tipi (�rn. Text, Procedure , function)</param>
        public void GetRecords(DataSet ds, string ssql, CommandType commandType)
        {
            if (ds == null)
                throw new ArgumentNullException("ds", "Dataset null olamaz");

            if (string.IsNullOrEmpty(ssql))
                throw new ArgumentNullException("ssql", "Bo� parametre verilemez.");

            if (ds.Tables.Count == 0)
                ds.Tables.Add();
            GetRecords(ds.Tables[0], ssql, commandType);
        }
        /// <summary>
        /// Select ��lemleri i�in kullan�l�r.
        /// </summary>
        /// <param name="ssql">�al��t�r�lacak komut.</param>
        /// <param name="commandType">�al��t�r�lacak olan komut tipi (�rn. Text, Procedure , function)</param>
        /// <returns>Sorgu sonucu doldurulan dataset</returns>
        public DataSet GetRecords(string ssql, CommandType commandType)
        {
            if (string.IsNullOrEmpty(ssql))
                throw new ArgumentNullException("ssql", "Bo� parametre verilemez.");

            DataTable dt = new DataTable();
            DataSet ds = new DataSet();
            ds.Tables.Add(dt);
            GetRecords(dt, ssql, commandType);
            return ds;
        }
        /// <summary>
        /// Select ��lemleri i�in kullan�l�r.
        /// </summary>
        /// <param name="ssql">�al��t�r�lacak komut.</param>
        /// <returns>Sorgu sonucu doldurulan dataset</returns>
        public DataSet GetRecords(string ssql)
        {
            if (string.IsNullOrEmpty(ssql))
                throw new ArgumentNullException("ssql", "Bo� parametre verilemez.");

            DataTable dt = new DataTable();
            DataSet ds = new DataSet();
            ds.Tables.Add(dt);
            GetRecords(dt, ssql, CommandType.Text);
            return ds;
        }
        /// <summary>
        /// Select ��lemleri i�in kullan�l�r.
        /// </summary>
        /// <param name="dt">Doldurulacak DataTable</param>
        /// <param name="ssql">�al��t�r�lacak komut.</param>
        /// <param name="commandType">�al��t�r�lacak olan komut tipi (�rn. Text, Procedure , function)</param>
        public void GetRecords(DataTable dt, string ssql, CommandType commandType)
        {
            if (dt == null)
                throw new ArgumentNullException("dt", "Datatable null olamaz");

            if (string.IsNullOrEmpty(ssql))
                throw new ArgumentNullException("ssql", "Bo� parametre verilemez.");

            this.OpenConnection();

            mDataAdapter.SelectCommand.CommandText = ssql;
            mDataAdapter.SelectCommand.Connection = mConnection;
            mDataAdapter.SelectCommand.CommandType = commandType;

            AddParameters(mDataAdapter.SelectCommand);

            try
            {
                if (mIsTransactional)
                    mDataAdapter.SelectCommand.Transaction = mTransaction;

                mDataAdapter.Fill(dt);
            }
            //catch (Exception e)
            //{
            //    ExceptionthrowHelper(e);
            //}
            finally
            {
                this.CloseConnection();
                mDataAdapter.SelectCommand.Transaction = null;
            }
        }
        /// <summary>
        /// Select ��lemleri i�in kullan�l�r.
        /// </summary>
        /// <param name="dt">Doldurulacak DataTable</param>
        /// <param name="ssql">�al��t�r�lacak komut.</param>
        /// <param name="withSchema">Tabloya ait �ema bilgisine ihtiya� duyulur ise bu parametre kullan�l�r</param>
        public void GetRecords(DataTable dt, string ssql, bool withSchema)
        {
            if (dt == null)
                throw new ArgumentNullException("dt", "Datatable null olamaz");

            if (string.IsNullOrEmpty(ssql))
                throw new ArgumentNullException("ssql", "Bo� parametre verilemez.");

            GetRecords(dt, ssql, CommandType.Text, withSchema);
        }
        /// <summary>
        /// Select ��lemleri i�in kullan�l�r.
        /// </summary>
        /// <param name="dt">Doldurulacak DataTable</param>
        /// <param name="ssql">�al��t�r�lacak komut.</param>
        /// <param name="commandType">�al��t�r�lacak olan komut tipi (�rn. Text, Procedure , function)</param>
        /// <param name="withSchema">Tabloya ait �ema bilgisine ihtiya� duyulur ise bu parametre kullan�l�r</param>
        public void GetRecords(DataTable dt, string ssql, CommandType commandType, bool withSchema)
        {
            if (dt == null)
                throw new ArgumentNullException("dt", "Datatable null olamaz");

            if (string.IsNullOrEmpty(ssql))
                throw new ArgumentNullException("ssql", "Bo� parametre verilemez.");

            this.OpenConnection();

            mDataAdapter.SelectCommand.CommandText = ssql;
            mDataAdapter.SelectCommand.Connection = mConnection;
            mDataAdapter.SelectCommand.CommandType = commandType;

            AddParameters(mDataAdapter.SelectCommand);

            if (withSchema)
                mDataAdapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;

            try
            {
                if (mIsTransactional)
                    mDataAdapter.SelectCommand.Transaction = mTransaction;

                mDataAdapter.Fill(dt);
            }
            //catch (Exception e)
            //{
            //    ExceptionthrowHelper(e);
            //}
            finally
            {
                this.CloseConnection();
                mDataAdapter.SelectCommand.Transaction = null;
            }
        }

        /// <summary>
        /// Parametre olarak verilen Dataset �zerinde yap�lan i�lemlerden sadece insert ve update 
        /// komutlar�n� �al��t�r�r.Delete komutu �al��t�r�lmaz.
        /// </summary>
        /// <param name="ds">�zerinde �al���lan dataset</param>
        public void InsertUpdate(DataSet ds)
        {
            if (ds == null)
                throw new ArgumentNullException("ds", "Dataset null olamaz");

            this.OpenConnection();

            mDataAdapter.SelectCommand.CommandText = "SELECT * FROM " + ds.Tables[0].TableName;
            mDataAdapter.SelectCommand.Connection = mConnection;

            if (IsTransactional)
                mDataAdapter.SelectCommand.Transaction = mTransaction;

            try
            {
                SqlCommandBuilder cb = new SqlCommandBuilder(mDataAdapter);

                mDataAdapter.InsertCommand = cb.GetInsertCommand();
                mDataAdapter.UpdateCommand = cb.GetUpdateCommand();
                mDataAdapter.DeleteCommand = null;

                if (IsTransactional)
                {
                    mDataAdapter.InsertCommand.Transaction = mTransaction;
                    mDataAdapter.UpdateCommand.Transaction = mTransaction;

                }

                mDataAdapter.Update(ds.Tables[0].Select(null, null, DataViewRowState.Added));
                mDataAdapter.Update(ds.Tables[0].Select(null, null, DataViewRowState.ModifiedCurrent));

            }
            //catch (Exception e)
            //{
            //    ExceptionthrowHelper(e);
            //}
            finally
            {
                this.CloseConnection();
                mDataAdapter.SelectCommand.Transaction = null;

                if (IsTransactional)
                {
                    mDataAdapter.InsertCommand.Transaction = null;
                    mDataAdapter.UpdateCommand.Transaction = null;
                    mDataAdapter.DeleteCommand.Transaction = null;
                }
            }
        }
        /// <summary>
        /// Dataset �zerinde yap�lan insert, update, delete i�lemlerini veritaban�na g�ndermek i�in kullan�l�r.
        /// </summary>
        /// <param name="ds">�zerinde i�lem yap�lan dataset</param>
        /// <param name="ssql">�al��t�r�lacak komut.</param>
        /// <param name="sTable">�zerinde �al���lan tablo ad�.</param>
        public void Update(DataSet ds, string ssql, string sTable)
        {
            if (ds == null)
                throw new ArgumentNullException("ds", "Dataset null olamaz");

            if (string.IsNullOrEmpty(ssql))
                throw new ArgumentNullException("ssql", "Bo� parametre verilemez.");

            if (string.IsNullOrEmpty(sTable))
                throw new ArgumentNullException("sTable", "Bo� parametre verilemez.");

            Update(ds.Tables[sTable], ssql);
        }
        /// <summary>
        /// Dataset �zerinde yap�lan insert, update, delete i�lemlerini veritaban�na g�ndermek i�in kullan�l�r.
        /// </summary>
        /// <param name="ds">�zerinde i�lem yap�lan dataset</param>
        public void Update(DataSet ds)
        {
            if (ds == null)
                throw new ArgumentNullException("ds", "Dataset null olamaz");

            string ssql = "SELECT * FROM " + ds.Tables[0].TableName;
            Update(ds.Tables[0], ssql);

        }
        /// <summary>
        /// Datatable �zerinde yap�lan insert, update, delete i�lemlerini veritaban�na g�ndermek i�in kullan�l�r.
        /// </summary>
        /// <param name="dt">�zerinde i�lem yap�lan Datatable</param>
        public void Update(DataTable dt)
        {
            if (dt == null)
                throw new ArgumentNullException("dt", "Datatable null olamaz");

            string ssql = "SELECT * FROM " + dt.TableName;
            Update(dt, ssql);
        }
        /// <summary>
        /// Datatable �zerinde yap�lan insert, update, delete i�lemlerini veritaban�na g�ndermek i�in kullan�l�r.
        /// </summary>
        /// <param name="dt">�zerinde i�lem yap�lan Datatable</param>
        /// <param name="ssql">�al��t�r�lacak komut.</param>
        private void Update(DataTable dt, string ssql)
        {
            if (dt == null)
                throw new ArgumentNullException("dt", "Datatable null olamaz");

            if (string.IsNullOrEmpty(ssql))
                throw new ArgumentNullException("ssql", "Bo� parametre verilemez.");

            this.OpenConnection();

            mDataAdapter.SelectCommand.CommandText = ssql;
            mDataAdapter.SelectCommand.Connection = mConnection;

            AddParameters(mDataAdapter.SelectCommand);

            if (IsTransactional)
                mDataAdapter.SelectCommand.Transaction = mTransaction;

            try
            {
                SqlCommandBuilder cb = new SqlCommandBuilder(mDataAdapter);

                if (IsTransactional)
                {
                    mDataAdapter.InsertCommand = cb.GetInsertCommand();
                    mDataAdapter.InsertCommand.Transaction = mTransaction;
                    mDataAdapter.UpdateCommand = cb.GetUpdateCommand();
                    mDataAdapter.UpdateCommand.Transaction = mTransaction;
                    mDataAdapter.DeleteCommand = cb.GetDeleteCommand();
                    mDataAdapter.DeleteCommand.Transaction = mTransaction;
                }
                mDataAdapter.Update(dt);
            }
            //catch (Exception e)
            //{

            //    ExceptionthrowHelper(e);
            //}
            finally
            {
                this.CloseConnection();
                mDataAdapter.SelectCommand.Transaction = null;

                if (IsTransactional)
                {
                    mDataAdapter.InsertCommand.Transaction = null;
                    mDataAdapter.UpdateCommand.Transaction = null;
                    mDataAdapter.DeleteCommand.Transaction = null;
                }
            }
        }

        /// <summary>
        /// Parametrelerin SqlCommand nesnesine eklemek i�in kullan�l�r.
        /// </summary>
        private void AddParameters(SqlCommand cmd)
        {

            foreach (SqlParameter p in parameters)
            {
                cmd.Parameters.Add(p);
            }

        }

        /// <summary>
        /// Output tipindeki SqlServer parametreleri eklemek i�in kullan�l�r.Output parametrelerde size olmak zorundad�r.
        /// </summary>
        /// <param name="parameterName">Parametre ad�.</param>
        /// <param name="dbType">SqlServer'a �zel parametre tipi.</param>
        /// <param name="direction">Parametre y�n�.</param>
        /// <param name="parameterSize">Parametre boyutu.</param>
        public void AddSqlParameter(string parameterName, SqlDbType dbType, ParameterDirection direction, int parameterSize)
        {
            if (string.IsNullOrEmpty(parameterName))
                throw new ArgumentNullException("parameterName", "Parametre ad� bo� olamaz.");

            if (parameterSize < 0)
                throw new ArgumentNullException("parameterSize", "parameterSize negatif de�er olamaz.");

            SqlParameter p = new SqlParameter();
            p.SqlDbType = dbType;
            p.ParameterName = parameterName;
            p.Direction = direction;
            p.Size = parameterSize;
            parameters.Add(p);
        }


        /// <summary>
        /// Input tipindeki SqlServer parametreleri eklemek i�in kullan�l�r.
        /// </summary>
        /// <param name="parameterName">Parametre ad�.</param>
        /// <param name="parameterValue">Veritaban�na eklenecek olan de�er.</param>
        /// <param name="dbType">SqlServer'a �zel parametre tipi.</param>
        /// <param name="parameterSize">Parametre boyutu.</param>
        public void AddSqlParameter(string parameterName, object parameterValue, SqlDbType dbType, int parameterSize)
        {
            if (string.IsNullOrEmpty(parameterName))
                throw new ArgumentNullException("parameterName", "Parametre ad� bo� olamaz.");

            if (parameterSize < 0)
                throw new ArgumentNullException("parameterSize", "parameterSize negatif de�er olamaz.");

            SqlParameter p = new SqlParameter();
            p.SqlDbType = dbType;
            p.ParameterName = parameterName;
            p.Value = parameterValue ?? DBNull.Value;
            p.Size = parameterSize;
            parameters.Add(p);
        }
        /// <summary>
        /// Output tipindeki Oracle parametreleri eklemek i�in kullan�l�r.Output parametrelerde size olmak zorundad�r.
        /// </summary>
        /// <param name="parameterName">Parametre ad�.</param>
        /// <param name="dbType">Oracle'a �zel parametre tipi.</param>
        /// <param name="direction">Parametre y�n�.</param>
        /// <param name="parameterSize">Parametren boyutu.</param>
        public void AddOracleParameter(string parameterName, OracleDbType dbType, ParameterDirection direction, int parameterSize)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Input tipindeki Oracle parametreleri eklemek i�in kullan�l�r.
        /// </summary>
        /// <param name="parameterName">Parametre ad�</param>
        /// <param name="parameterValue">Parametrenin de�eri.</param>
        /// <param name="dbType">Oracle'a �zel parametre tipi.</param>
        /// <param name="parameterSize">Parametren boyutu.</param>
        public void AddOracleParameter(string parameterName, object parameterValue, Oracle.DataAccess.Client.OracleDbType dbType, int parameterSize)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Veritaban� i�lemleri bittikten sonra SqlCommand nesnesi i�indeki parametreleri� temizler.
        /// </summary>
        private void ClearCommandParameters()
        {
            parameters.Clear();
            mCommand.Parameters.Clear();

        }
        /// <summary>
        /// Olu�an hata ile ilgli daha fazla bilgi alabilmek i�in hatay� d�zenler.
        /// </summary>
        /// <param name="ex"></param>
        private void ExceptionthrowHelper(Exception ex)
        {

            ex.Data.Add("Hata Tipi :", ex.GetBaseException());
            ex.Data.Add("Hata Kayna�� :", ex.Source);
            ex.Data.Add("Hata Mesaj� :", ex.Message);
            ex.Data.Add("Sat�r :", ex.StackTrace.Contains("line") ? ex.StackTrace.Substring(ex.StackTrace.IndexOf("line", System.StringComparison.Ordinal)) : ex.StackTrace.Substring(ex.StackTrace.IndexOf("konum", System.StringComparison.Ordinal)));

            ex.Data.Add("G�nderilen Sql Komutu:", mCommand.CommandText);
            ex.Data.Add("metod ad� :", ex.StackTrace.Contains(" in ") ? ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(" in ", System.StringComparison.Ordinal)) : ex.StackTrace.Substring(ex.StackTrace.LastIndexOf("i�inde", System.StringComparison.Ordinal)));
            ex.Data.Add("Parametreler :", null);

            foreach (var item in parameters)
            {

                ex.Data.Add(item.ParameterName, item.Value);
            }

            ex.Data.Add("Stack :", ex.StackTrace);


            throw ex;

        }
    }
}