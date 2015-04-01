using System;
using System.Diagnostics;
using WebFrame.DataAccess;
using WebFrame.DataType.Common.Attributes;
using WebFrame.DataType.Common.Logging;
using WebFrame.DataType.Common.Enums;
using System.Data;
using NLog;

namespace WebFrame.Business
{
    /// <summary>
    /// 
    /// </summary>
    public class BusinessBase
    {
       
        Logger nlogLogger;
        protected IData mDataObject { get; set; }
       // protected bool mIsRoot = false;
        /// <summary>
        /// 
        /// </summary>
        public bool mIsRoot { get; set; }
        ServiceConnectionNameAttribute serviceAttribute;


        private int nestedLevel = 0;
        /// <summary>
        /// 
        /// </summary>
        public BusinessBase()
        {

            mDataObject = null;
            mIsRoot = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected bool IsRoot()
        {

            if (nestedLevel > 0)
            {
                nestedLevel--;
                return false;
            }
            return mIsRoot;

        }


        /// <summary>
        /// Bu property; farkl� tablespace veya farkl� veritabanlar� �zerinde yap�lan i�lemleri tek bir transaction alt�nda birle�tirmek amac�yla eklenmi�tir.
        /// �rnek i�in KullaniciBS.SifreDegisikligi metoduna bakabilirsiniz. �lgili metodta iki farkl� Oracle tablespace ve birde sqlserver
        /// ayn� transaction alt�nda kullan�lm�� ve herhangi bir hata olursa b�t�n veritabanlar�ndaki i�lemler geri al�nabilmi�tir.
        /// </summary>
        public IData CurrentDataObject
        {
            get { return mDataObject; }
        }

        /// <summary>
        /// Log i�lemleri i�in kullan�l�r. E�er kendi loglama i�leminizi yapmak istiyorsan�z <see cref="ILogger"/> interface kullanarak
        /// kendi s�n�f�n�z� olu�turabilirsiniz. Bu property kendi loglama s�n�flar�n� kullanman�za olanak sa�lar.
        /// </summary>
        public ILogger Logger { get; set; }

        /// <summary>
        /// NLog altyap�s�n� kullanarak loglama sa�lar. Konfig�rasyon dosyas�na istenilen ayarlar eklenerek
        /// istenilen kayna�a (dosya, veritaban� vs.) loglama yap�labilir.
        /// </summary>
        public Logger NLogLogger
        {
            get
            {
                if(nlogLogger==null)
                    nlogLogger=LogManager.GetCurrentClassLogger();

                return nlogLogger;
            }
            set
            {
                nlogLogger=value;
            }
        }
        /// <summary>
        /// �lgili business classa ait ba�lant� bilgilerini alan metod
        /// </summary>
        /// <returns>Ba�lant� bilgilerini i�eren class</returns>
        private ServiceConnectionNameAttribute GetServiceConfiguration()
        {
            if (this.GetType().IsDefined(typeof(ServiceConnectionNameAttribute), false))
            {
                serviceAttribute = (ServiceConnectionNameAttribute)this.GetType().GetCustomAttributes(typeof(ServiceConnectionNameAttribute), false)[0];
            }

            return serviceAttribute;
        }

        /// <summary>
        /// �zerinde �al���lan projeye ait instance d�nd�r�r (�rn. BilgeUserBS)
        /// </summary>
        /// <param name="connectionStringName">Ba�lant� ad�</param>
        /// <param name="provider">Hangi veritaban�na ba�lan�laca��</param>
        /// <returns>Oracle veya SqlServer veritaban�yla �al��abilen projeye ait nesne (�rn. BilgeUserBS)</returns>
        protected IData GetDataObject(string connectionStringName, DataProvider provider)
        {
            if (string.IsNullOrEmpty(connectionStringName))
                return this.GetDataObject();

            if (mDataObject == null)
            {
                mDataObject = DataFactory.GetDataObject(connectionStringName, provider);
                if (mDataObject.IsTransactional)
                    mIsRoot = true;
            }
            else
            {
                //ayni BS ten kendi iclerinde transaction olan 2 farkli metodun icice cagrildigi durumlar icin eklendi.
                if (mDataObject.IsTransactional && mIsRoot)
                {
                    nestedLevel++;

                    return mDataObject;

                }
                if (mDataObject.IsTransactional)
                {

                    nestedLevel++;

                    mIsRoot = true;
                    return mDataObject;

                }
                else if (mDataObject.IsTransactional && !mIsRoot)
                {
                    return mDataObject;
                }
                else
                {
                    mDataObject = DataFactory.GetDataObject(connectionStringName, provider);
                    if (mDataObject.IsTransactional)
                        mIsRoot = true;
                }
            }
            return mDataObject;

        }

        /// <summary>
        /// �zerinde �al���lan projeye ait instance d�nd�r�r (�rn. BilgeUserBS)
        /// </summary>
        /// <param name="connectionStringName">Ba�lant� ad�</param>
        /// <returns>Oracle veya SqlServer veritaban�yla �al��abilen projeye ait nesne (�rn. BilgeUserBS)</returns>
        protected IData GetDataObject(string connectionStringName)
        {

            if (string.IsNullOrEmpty(connectionStringName))
                return this.GetDataObject();

            if (mDataObject == null)
            {
                mDataObject = DataFactory.GetDataObject(connectionStringName, GetServiceConfiguration().DataProvider);
                if (mDataObject.IsTransactional)
                    mIsRoot = true;
            }
            else
            {
                //ayni BS ten kendi iclerinde transaction olan 2 farkli metodun icice cagrildigi durumlar icin eklendi.
                if (mDataObject.IsTransactional && mIsRoot)
                {
                    nestedLevel++;

                    return mDataObject;

                }
                if (mDataObject.IsTransactional)
                {

                    nestedLevel++;

                    mIsRoot = true;
                    return mDataObject;

                }
                else if (mDataObject.IsTransactional && !mIsRoot)
                {
                    return mDataObject;
                }
                else
                {
                    mDataObject = DataFactory.GetDataObject(connectionStringName, GetServiceConfiguration().DataProvider);
                    if (mDataObject.IsTransactional)
                        mIsRoot = true;
                }
            }
            return mDataObject;

        }

        /// <summary>
        /// �zerinde �al���lan projeye ait instance d�nd�r�r (�rn. BilgeUserBS)
        /// </summary>
        /// <returns>Oracle veya SqlServer veritaban�yla �al��abilen projeye ait nesne (�rn. BilgeUserBS)</returns>
        protected IData GetDataObject()
        {

            string connection = "";
            if (this.GetType().IsDefined(typeof(ServiceConnectionNameAttribute), false))
            {
                ServiceConnectionNameAttribute connectionNameAtt = (ServiceConnectionNameAttribute)this.GetType().GetCustomAttributes(typeof(ServiceConnectionNameAttribute), false)[0];
                connection = connectionNameAtt.ServiceConnectionName;
            }

            return this.GetDataObject(connection);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected T CreateObject<T>()
        {
            T oBusinessObject = default(T);
            oBusinessObject = Activator.CreateInstance<T>();
            if ((mDataObject != null) && (mDataObject.IsTransactional))
            {
                BusinessBase b = oBusinessObject as BusinessBase;
                b.mDataObject = this.mDataObject;
                // b.mIsRoot = false;
            }
            return oBusinessObject;
        }

        #region "Exception Handling"

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ex"></param>
        protected void HandleSystemException(Exception ex)
        {
            HandleGumrukWebException<Exception>(ex);
          
        }

        /// <summary>
        /// Olu�an hatalar�n yakalan�p i�lenmesi i�in kullan�lan metod
        /// </summary>
        /// <typeparam name="T">Exception tipinden bir nesne</typeparam>
        /// <param name="ex">GumrukWebException tipinden bir nesne</param>
        protected void HandleGumrukWebException<T>(T ex) where T : Exception
        {
            //1. burada hata loglanabilir
            //2. hata bilgileri veri taban�ndan cekilip ona gore kullan�c�ya bilgi g�sterilebilir AYKD_TANIM
            //3. tan�ma ili�kin eylem varsa aksiyon almak icin SMS atma email atma i�lemleri yap�labilir.

            //GumrukApplicationException resultEx;
            //if (ex is GumrukApplicationException)
            //{
            //    resultEx = (GumrukApplicationException)Convert.ChangeType(ex, typeof(GumrukApplicationException));
            //}
            //else if (ex is SQLKomutCalismaHatasi)
            //{
            //    GumrukApplicationException e = new GumrukApplicationException();
            //    e.ExceptionType = EnumExceptionType.DBEXCEPTION;
            //    e.ErrorExternalMessage = ex.Message;
            //    e.ErrorInternalMessage = "Komut �al��t�rma hatas�";
            //    e.ErrorCode = "DB";
            //    resultEx = e;
            //}
            //else if (ex is VeritabaniBaglantiHatasi)
            //{
            //    GumrukApplicationException e = new GumrukApplicationException();
            //    e.ExceptionType = EnumExceptionType.DBEXCEPTION;
            //    e.ErrorExternalMessage = ex.Message ;
            //    e.ErrorInternalMessage = "Veritaban� hatas�";
            //    e.ErrorCode = "DB";
            //    resultEx = e;

            //}
            //else if (ex is ApplicationException)
            //{
            //    GumrukApplicationException e = new GumrukApplicationException();
            //    e.ExceptionType = EnumExceptionType.DBEXCEPTION;
            //    e.ErrorExternalMessage = ex.Message;
            //    e.ErrorInternalMessage = "Sunucu uygulama hatas�";
            //    e.ErrorCode = "APPSRV";
            //    resultEx = e;

            //}
            //else
            //{
            //    GumrukApplicationException e = new GumrukApplicationException();
            //    e.ExceptionType = EnumExceptionType.SYSTEMEXCEPTION;
            //    e.ErrorExternalMessage = ex.ToString();
            //    e.ErrorInternalMessage = "Bilinmeyen hata";
            //    e.ErrorCode = "UNKNOWN";
            //    resultEx = e;

            //}

            //LogWriter logger = new LogWriter();
            //logger.Write(GumrukModules.Genel, 0, EventLogEntryType.Error, this.GetType().FullName, ex.Message);

            throw ex;

        }

        /// <summary>
        /// Olu�an hatay� veritaban�na yazar.E�er veritaban�na yaz�lam�yorsa i�letim sistemine ait eventlog tablosuna yazar.
        /// </summary>
        /// <param name="moduleId">Proje ad�</param>
        /// <param name="eventType">Loga yaz�lacak i�lemin tipi</param>
        /// <param name="ex">Olu�an hata (Exception)</param>
        /// <param name="pageUrl">Hatan�n olu�tu�u sayfa</param>
        /// <param name="methodName">Hatan�n olu�tu�u metod</param>
        /// <param name="message">hata ile ilgili mesaj</param>
        /// <param name="yetkiler">Varsa ki�iye ait yetkiler</param>
        /// <param name="extraParameters">Varsa extra parametreler</param>
        protected void Log(AppModules moduleId, EventLogEntryType eventType, Exception ex, string pageUrl, string methodName, string message, DataSet yetkiler, params string[] extraParameters)
        {
            Log(moduleId, eventType, ex,pageUrl, methodName, message, "", "", yetkiler, extraParameters);
        }

        /// <summary>
        /// Olu�an hatay� veritaban�na yazar.E�er veritaban�na yaz�lam�yorsa i�letim sistemine ait eventlog tablosuna yazar.
        /// </summary>
        /// <param name="moduleId">Proje ad�</param>
        /// <param name="eventType">Loga yaz�lacak i�lemin tipi</param>
        /// <param name="ex">Olu�an hata (Exception)</param>
        /// <param name="pageUrl">Hatan�n olu�tu�u sayfa</param>
        /// <param name="methodName">Hatan�n olu�tu�u metod</param>
        /// <param name="message">hata ile ilgili mesaj</param>
        /// <param name="extraParameters">Varsa extra parametreler</param>
        protected void Log(AppModules moduleId, EventLogEntryType eventType, Exception ex, string pageUrl, string methodName, string message, params string[] extraParameters)
        {
            Log(moduleId, eventType, ex, pageUrl, methodName, message, "", "", null, extraParameters);

        }

        /// <summary>
        /// Olu�an hatay� veritaban�na yazar.E�er veritaban�na yaz�lam�yorsa i�letim sistemine ait eventlog tablosuna yazar.
        /// </summary>
        /// <param name="moduleId">Proje ad�</param>
        /// <param name="eventType">Loga yaz�lacak i�lemin tipi</param>
        /// <param name="ex">Olu�an hata (Exception)</param>
        /// <param name="pageUrl">Hatan�n olu�tu�u sayfa</param>
        /// <param name="methodName">Hatan�n olu�tu�u metod</param>
        /// <param name="message">hata ile ilgili mesaj</param>
        /// <param name="kullaniciSicil">Varsa kullan�c�n�n sicili</param>
        /// <param name="pcName">��lemi yapan kullan�c�ya ait pc ad�</param>
        /// <param name="yetkiler">Varsa ki�iye ait yetkiler</param>
        /// <param name="extraParameters">Varsa extra parametreler</param>
        protected void Log(AppModules moduleId, EventLogEntryType eventType, Exception ex, string pageUrl, string methodName, string message, string kullaniciSicil, string pcName, DataSet yetkiler, params string[] extraParameters)
        {
            if(Logger==null)
                Logger = new LogWriter(GetServiceConfiguration().DataProvider);

            Logger.Write(moduleId, eventType, ex, pageUrl, methodName, message, kullaniciSicil, pcName, yetkiler, extraParameters, "");
        }

        /// <summary>
        /// Tablo �zerinde yap�lan i�lemlerin(insert ,update ,delete) loglanmas� amac�yla haz�rlanm��t�r.
        /// </summary>
        /// <param name="tableName">Tablo Ad�</param>
        /// <param name="rowid">Veriye ait ID</param>
        /// <param name="columnName">S�tun Ad�</param>
        /// <param name="operation">Tablo �zerinde yap�lan i�lem</param>
        /// <param name="user">��lemi yapan kullan�c�</param>
        /// <param name="projectname">�zerinde �al���lan proje ad�</param>
        /// <param name="oldvalue">tablodaki eski de�er</param>
        /// <param name="newvalue">tablodaki yeni de�er</param>
        /// <param name="message">Varsa i�lem ile ilgili mesaj</param>
        protected void LogAudit(string tableName, string rowid, string columnName, TableOperations operation, string user, string projectname,string oldvalue,string newvalue,string message)
        {

            if (Logger == null)
                Logger = new LogWriter(GetServiceConfiguration().DataProvider);

            Logger.WriteAudit(tableName, rowid, columnName, operation, user, projectname, oldvalue, newvalue, message);
        }

        #endregion
    }
}