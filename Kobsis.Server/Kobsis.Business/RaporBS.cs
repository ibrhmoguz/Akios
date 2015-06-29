﻿using System;
using System.Data;
using WebFrame.Business;
using WebFrame.DataAccess;
using WebFrame.DataType.Common.Attributes;

namespace Kobsis.Business
{
    [ServiceConnectionName("KobsisConnectionString")]
    public class RaporBS : BusinessBase
    {
        public DataTable GunlukIsTakipFormuListele(DateTime raporTarihi, string musteriKod)
        {
            DataTable dt = new DataTable();
            IData data = GetDataObject();
            string sqlText = @"SELECT
                                ROW_NUMBER() OVER(ORDER BY S.SiparisNo DESC) AS ID
                                , S.SiparisNo
                                , S.MusteriAd + ' ' + S.MusteriSoyad AS Musteri
                                ,  ISNULL(CASE WHEN S.MusteriCepTel IS NOT NULL THEN 'CEP: '+ S.MusteriCepTel ELSE NULL END,'') + ' ' +
                                   ISNULL(CASE WHEN S.MusteriEvTel IS NOT NULL THEN 'EV: '+ S.MusteriEvTel ELSE NULL END,'') + ' ' +
                                   ISNULL(CASE WHEN S.MusteriIsTel IS NOT NULL THEN 'İŞ: '+ S.MusteriIsTel ELSE NULL END,'') AS Tel
                                , S.MusteriAdres AS Adres
                                , RI.IlAd +'/'+ RC.IlceAd + '/'+ RS.SemtAd AS Semt
                                , dbo.TESLIMAT_EKIP_LISTESI(M.ID) AS TeslimatEkibi
                                --, ISNULL(CASE WHEN S.KILITSISTEM IS NOT NULL THEN S.KILITSISTEM +', ' ELSE NULL END,'')+
                                --  ISNULL(CASE WHEN S.CITA IS NOT NULL THEN S.CITA +',  ' ELSE NULL END,'') +
                                --  ISNULL(CASE WHEN S.ESIK IS NOT NULL THEN S.ESIK +',' ELSE NULL END,'') +
                                --  ISNULL(CASE WHEN S.ALUMINYUMRENK IS NOT NULL THEN S.ALUMINYUMRENK +', ' ELSE NULL END,'') +
                                --  ISNULL(CASE WHEN S.CONTARENK IS NOT NULL THEN S.CONTARENK +', ' ELSE NULL END,'') +
                                --  ISNULL(CASE WHEN S.TACTIP IS NOT NULL THEN S.TACTIP +', ' ELSE NULL END,'') +
                                --  ISNULL(CASE WHEN S.PERVAZTIP IS NOT NULL THEN S.PERVAZTIP +', ' ELSE NULL END,'') +
                                --  ISNULL(CASE WHEN S.CEKMEKOLU IS NOT NULL THEN S.CEKMEKOLU +', ' ELSE NULL END,'') +
                                --  ISNULL(CASE WHEN S.BARELTIP IS NOT NULL THEN S.BARELTIP +', ' ELSE NULL END,'')+ 
                                --  ISNULL(CASE WHEN S.BABA IS NOT NULL THEN 'Baba:'+S.BABA +', ' ELSE NULL END,'') +
                                --  ISNULL(CASE WHEN S.DURBUN IS NOT NULL THEN 'Dürbün:'+S.DURBUN +', ' ELSE NULL END,'') +
                                --  ISNULL(CASE WHEN S.TAKTAK IS NOT NULL THEN 'Taktak:'+S.TAKTAK +', ' ELSE NULL END,'') +
                                --  ISNULL(CASE WHEN S.KAYITSIZKAMERA IS NOT NULL THEN 'Kayıtsız Kamera:'+S.KAYITSIZKAMERA +', ' ELSE NULL END,'') +
                                --  ISNULL(CASE WHEN S.KAYITYAPANKAMERA IS NOT NULL THEN  'Kayıt Yapan Kamera:'+S.KAYITYAPANKAMERA +', ' ELSE NULL END,'') +
                                --  ISNULL(CASE WHEN S.ALARM IS NOT NULL THEN 'Alarm:'+S.ALARM +', ' ELSE NULL END,'') +
                                --  ISNULL(CASE WHEN S.OTOKILIT IS NOT NULL THEN 'Otomatik Kilit:'+S.OTOKILIT +', ' ELSE NULL END,'') AS ACIKLAMA
                            FROM dbo.SIPARIS_{0} AS S
                                INNER JOIN TESLIMAT AS M ON M.SiparisID = S.ID
                                LEFT OUTER JOIN dbo.REF_SEMT AS RS ON RS.SemtKod = S.MusteriSemtKod
                                INNER JOIN dbo.REF_ILCE AS RC ON RC.IlceKod = S.MusteriIlceKod
                                INNER JOIN dbo.REF_IL AS RI ON RI.IlKod = S.MusteriIlKod
                            WHERE CONVERT(DATE, CONVERT(VARCHAR(24),M.TeslimTarih,103),103)= @TeslimTarih";
            
            sqlText = String.Format(sqlText, musteriKod);
            data.AddSqlParameter("TeslimTarih", raporTarihi, SqlDbType.Date, 50);
            data.GetRecords(dt, sqlText);

            return dt;
        }

        public DataSet SeriyeGoreSatilanAdet(string il, string ilce, string yil, string musteriKod)
        {
            DataSet ds = new DataSet();
            ds.Tables.Add(SeriyeGoreSatilan(il, ilce, yil, musteriKod));
            ds.Tables.Add(SeriyeGoreSatilanTutar(il, ilce, yil, musteriKod));

            return ds;
        }

        private DataTable SeriyeGoreSatilanTutar(string il, string ilce, string yil, string musteriKod)
        {
            DataTable dt = new DataTable();
            IData data = GetDataObject();
            string sqlText = @"DECLARE @Period AS DATE;
                                SET @Period = CAST('01.01.' + @Yil AS DATE);
                                WITH KAPI_FILITRELE AS 
                                (
	                                SELECT
		                                 KS.AD AS KapiTipi
		                                 , S.FIYAT
		                                 ,  CAST(S.SIPARISTARIH AS DATE) AS SIPARISTARIH
	                                FROM dbo.SIPARIS_{0} AS S
		                                INNER JOIN dbo.REF_KAPISERI_HEPSI AS KS ON SUBSTRING(S.SIPARISNO, 1, 1) = KS.VALUE 
	                                WHERE (@Il IS NULL OR MUSTERIIL = @Il) AND
		                                  (@Ilce IS NULL OR MUSTERIILCE = @Ilce) AND
		                                  (@Yil IS NULL OR DATEPART(YEAR,SIPARISTARIH) = @Yil)
                                )

                                SELECT
	                                KapiTipi AS [TOPLAM TUTAR]
	                                , ISNULL((SELECT SUM(ISNULL(FIYAT,'0')) FROM KAPI_FILITRELE WHERE SIPARISTARIH >= @Period AND SIPARISTARIH < DATEADD(MONTH,1,@Period) AND KapiTipi=KF.KapiTipi),'0') AS '1'
	                                , ISNULL((SELECT SUM(ISNULL(FIYAT,'0')) FROM KAPI_FILITRELE WHERE SIPARISTARIH >= DATEADD(MONTH,1,@Period) AND SIPARISTARIH < DATEADD(MONTH,2,@Period) AND KapiTipi=KF.KapiTipi),'0') AS '2'
	                                , ISNULL((SELECT SUM(ISNULL(FIYAT,'0')) FROM KAPI_FILITRELE WHERE SIPARISTARIH >= DATEADD(MONTH,2,@Period) AND SIPARISTARIH < DATEADD(MONTH,3,@Period) AND KapiTipi=KF.KapiTipi),'0') AS '3'
	                                , ISNULL((SELECT SUM(ISNULL(FIYAT,'0')) FROM KAPI_FILITRELE WHERE SIPARISTARIH >= DATEADD(MONTH,3,@Period) AND SIPARISTARIH < DATEADD(MONTH,4,@Period) AND KapiTipi=KF.KapiTipi),'0') AS '4'
	                                , ISNULL((SELECT SUM(ISNULL(FIYAT,'0')) FROM KAPI_FILITRELE WHERE SIPARISTARIH >= DATEADD(MONTH,4,@Period) AND SIPARISTARIH < DATEADD(MONTH,5,@Period) AND KapiTipi=KF.KapiTipi),'0') AS '5'
	                                , ISNULL((SELECT SUM(ISNULL(FIYAT,'0')) FROM KAPI_FILITRELE WHERE SIPARISTARIH >= DATEADD(MONTH,5,@Period) AND SIPARISTARIH < DATEADD(MONTH,6,@Period) AND KapiTipi=KF.KapiTipi),'0') AS '6'
	                                , ISNULL((SELECT SUM(ISNULL(FIYAT,'0')) FROM KAPI_FILITRELE WHERE SIPARISTARIH >= DATEADD(MONTH,6,@Period) AND SIPARISTARIH < DATEADD(MONTH,7,@Period) AND KapiTipi=KF.KapiTipi),'0') AS '7'
	                                , ISNULL((SELECT SUM(ISNULL(FIYAT,'0')) FROM KAPI_FILITRELE WHERE SIPARISTARIH >= DATEADD(MONTH,7,@Period) AND SIPARISTARIH < DATEADD(MONTH,8,@Period) AND KapiTipi=KF.KapiTipi),'0') AS '8'
	                                , ISNULL((SELECT SUM(ISNULL(FIYAT,'0')) FROM KAPI_FILITRELE WHERE SIPARISTARIH >= DATEADD(MONTH,8,@Period) AND SIPARISTARIH < DATEADD(MONTH,9,@Period) AND KapiTipi=KF.KapiTipi),'0') AS '9'
	                                , ISNULL((SELECT SUM(ISNULL(FIYAT,'0')) FROM KAPI_FILITRELE WHERE SIPARISTARIH >= DATEADD(MONTH,9,@Period) AND SIPARISTARIH < DATEADD(MONTH,10,@Period) AND KapiTipi=KF.KapiTipi),'0') AS '10'
	                                , ISNULL((SELECT SUM(ISNULL(FIYAT,'0')) FROM KAPI_FILITRELE WHERE SIPARISTARIH >= DATEADD(MONTH,10,@Period) AND SIPARISTARIH < DATEADD(MONTH,11,@Period) AND KapiTipi=KF.KapiTipi),'0') AS '11'
	                                , ISNULL((SELECT SUM(ISNULL(FIYAT,'0')) FROM KAPI_FILITRELE WHERE SIPARISTARIH >= DATEADD(MONTH,11,@Period) AND SIPARISTARIH < DATEADD(MONTH,12,@Period) AND KapiTipi=KF.KapiTipi),'0') AS '12'
                                    , ISNULL((SELECT SUM(ISNULL(FIYAT,'0')) FROM KAPI_FILITRELE WHERE SIPARISTARIH >= @Period AND SIPARISTARIH < DATEADD(MONTH,12,@Period) AND KapiTipi=KF.KapiTipi),'0') AS 'Yillik'
                                    , '' AS [Yuzde(%)]
                                FROM KAPI_FILITRELE AS KF
                                GROUP BY KapiTipi";
            
            sqlText = String.Format(sqlText, musteriKod);

            data.AddSqlParameter("Il", il, SqlDbType.VarChar, 50);
            data.AddSqlParameter("Ilce", ilce, SqlDbType.VarChar, 50);
            data.AddSqlParameter("Yil", yil, SqlDbType.VarChar, 50);
            data.GetRecords(dt, sqlText);

            return dt;
        }

        private DataTable SeriyeGoreSatilan(string il, string ilce, string yil, string musteriKod)
        {
            DataTable dt = new DataTable();
            IData data = GetDataObject();
            string sqlText = @"DECLARE @Period AS DATE;
                                SET @Period = CAST('01.01.' + @Yil AS DATE);
                                WITH KAPI_FILITRELE AS 
                                (
	                                SELECT
		                                 KS.AD AS KapiTipi
		                                 , S.ADET
		                                 ,  CAST(S.SIPARISTARIH AS DATE) AS SIPARISTARIH
	                                FROM dbo.SIPARIS_{0} AS S
		                                INNER JOIN dbo.REF_KAPISERI_HEPSI AS KS ON SUBSTRING(S.SIPARISNO, 1, 1) = KS.VALUE 
	                                WHERE (@Il IS NULL OR MUSTERIIL = @Il) AND
		                                  (@Ilce IS NULL OR MUSTERIILCE = @Ilce) AND
		                                  (@Yil IS NULL OR DATEPART(YEAR,SIPARISTARIH) = @Yil)
                                )

                                SELECT
	                                KapiTipi AS [TOPLAM SATIŞ]
	                                , CAST(ISNULL((SELECT SUM(CAST(ISNULL(ADET,'0') AS INT)) FROM KAPI_FILITRELE WHERE SIPARISTARIH >= @Period AND SIPARISTARIH < DATEADD(MONTH,1,@Period) AND KapiTipi=KF.KapiTipi),0) AS VARCHAR) AS '1'
	                                , CAST(ISNULL((SELECT SUM(CAST(ISNULL(ADET,'0') AS INT)) FROM KAPI_FILITRELE WHERE SIPARISTARIH >= DATEADD(MONTH,1,@Period) AND SIPARISTARIH < DATEADD(MONTH,2,@Period) AND KapiTipi=KF.KapiTipi),0) AS VARCHAR) AS '2'
	                                , CAST(ISNULL((SELECT SUM(CAST(ISNULL(ADET,'0') AS INT)) FROM KAPI_FILITRELE WHERE SIPARISTARIH >= DATEADD(MONTH,2,@Period) AND SIPARISTARIH < DATEADD(MONTH,3,@Period) AND KapiTipi=KF.KapiTipi),0) AS VARCHAR) AS '3'
	                                , CAST(ISNULL((SELECT SUM(CAST(ISNULL(ADET,'0') AS INT)) FROM KAPI_FILITRELE WHERE SIPARISTARIH >= DATEADD(MONTH,3,@Period) AND SIPARISTARIH < DATEADD(MONTH,4,@Period) AND KapiTipi=KF.KapiTipi),0) AS VARCHAR) AS '4'
	                                , CAST(ISNULL((SELECT SUM(CAST(ISNULL(ADET,'0') AS INT)) FROM KAPI_FILITRELE WHERE SIPARISTARIH >= DATEADD(MONTH,4,@Period) AND SIPARISTARIH < DATEADD(MONTH,5,@Period) AND KapiTipi=KF.KapiTipi),0) AS VARCHAR) AS '5'
	                                , CAST(ISNULL((SELECT SUM(CAST(ISNULL(ADET,'0') AS INT)) FROM KAPI_FILITRELE WHERE SIPARISTARIH >= DATEADD(MONTH,5,@Period) AND SIPARISTARIH < DATEADD(MONTH,6,@Period) AND KapiTipi=KF.KapiTipi),0) AS VARCHAR) AS '6'
	                                , CAST(ISNULL((SELECT SUM(CAST(ISNULL(ADET,'0') AS INT)) FROM KAPI_FILITRELE WHERE SIPARISTARIH >= DATEADD(MONTH,6,@Period) AND SIPARISTARIH < DATEADD(MONTH,7,@Period) AND KapiTipi=KF.KapiTipi),0) AS VARCHAR) AS '7'
	                                , CAST(ISNULL((SELECT SUM(CAST(ISNULL(ADET,'0') AS INT)) FROM KAPI_FILITRELE WHERE SIPARISTARIH >= DATEADD(MONTH,7,@Period) AND SIPARISTARIH < DATEADD(MONTH,8,@Period) AND KapiTipi=KF.KapiTipi),0) AS VARCHAR) AS '8'
	                                , CAST(ISNULL((SELECT SUM(CAST(ISNULL(ADET,'0') AS INT)) FROM KAPI_FILITRELE WHERE SIPARISTARIH >= DATEADD(MONTH,8,@Period) AND SIPARISTARIH < DATEADD(MONTH,9,@Period) AND KapiTipi=KF.KapiTipi),0) AS VARCHAR) AS '9'
	                                , CAST(ISNULL((SELECT SUM(CAST(ISNULL(ADET,'0') AS INT)) FROM KAPI_FILITRELE WHERE SIPARISTARIH >= DATEADD(MONTH,9,@Period) AND SIPARISTARIH < DATEADD(MONTH,10,@Period) AND KapiTipi=KF.KapiTipi),0) AS VARCHAR) AS '10'
	                                , CAST(ISNULL((SELECT SUM(CAST(ISNULL(ADET,'0') AS INT)) FROM KAPI_FILITRELE WHERE SIPARISTARIH >= DATEADD(MONTH,10,@Period) AND SIPARISTARIH < DATEADD(MONTH,11,@Period) AND KapiTipi=KF.KapiTipi),0) AS VARCHAR) AS '11'
	                                , CAST(ISNULL((SELECT SUM(CAST(ISNULL(ADET,'0') AS INT)) FROM KAPI_FILITRELE WHERE SIPARISTARIH >= DATEADD(MONTH,11,@Period) AND SIPARISTARIH < DATEADD(MONTH,12,@Period) AND KapiTipi=KF.KapiTipi),0) AS VARCHAR) AS '12'
                                    , CAST(ISNULL((SELECT SUM(CAST(ISNULL(ADET,'0') AS INT)) FROM KAPI_FILITRELE WHERE SIPARISTARIH >= @Period AND SIPARISTARIH < DATEADD(MONTH,12,@Period) AND KapiTipi=KF.KapiTipi),0) AS VARCHAR) AS 'Yillik'
                                    , '' AS [Yuzde(%)]
                                FROM KAPI_FILITRELE AS KF
                                GROUP BY KapiTipi";

            sqlText = String.Format(sqlText, musteriKod);

            data.AddSqlParameter("Il", il, SqlDbType.VarChar, 50);
            data.AddSqlParameter("Ilce", ilce, SqlDbType.VarChar, 50);
            data.AddSqlParameter("Yil", yil, SqlDbType.VarChar, 50);
            data.GetRecords(dt, sqlText);

            return dt;
        }

        public DataSet IlIlceyeGoreSatilanAdet(string il, string ilce, string yil)
        {
            DataSet ds = new DataSet();
            if (string.IsNullOrEmpty(il))
            {
                DataTable dtIl = IlSatilanAdetGetir(il, yil);
                ds.Tables.Add(dtIl);
                ds.Tables.Add(new DataTable());
                ds.Tables.Add(new DataTable());
            }
            else if (string.IsNullOrEmpty(ilce))
            {
                DataTable dtIl = IlSatilanAdetGetir(il, yil);
                DataTable dtIlce = IlceSatilanAdetGetir(il, ilce, yil);
                ds.Tables.Add(dtIl);
                ds.Tables.Add(dtIlce);
                ds.Tables.Add(new DataTable());
            }
            else
            {
                DataTable dtIl = IlSatilanAdetGetir(il, yil);
                DataTable dtIlce = IlceSatilanAdetGetir(il, ilce, yil);
                DataTable dtSemt = SemtSatilanAdetGetir(il, ilce, yil);
                ds.Tables.Add(dtIl);
                ds.Tables.Add(dtIlce);
                ds.Tables.Add(dtSemt);
            }

            return ds;
        }

        private DataTable IlSatilanAdetGetir(string il, string yil)
        {
            DataTable dt = new DataTable();
            IData data = GetDataObject();
            string sqlText = @"DECLARE @Period AS DATE;
                                SET @Period = CAST('01.01.' + @Yil AS DATE);

                                WITH KAPI_FILITRELE AS 
                                (
	                                SELECT
		                                MUSTERIIL AS Il
		                                 , MUSTERIILCE AS IlIlce
		                                 , MUSTERISEMT AS Semt
		                                 , ADET
		                                 , CAST(SIPARISTARIH AS DATE) AS SIPARISTARIH
	                                FROM dbo.SIPARIS
	                                WHERE (@Il IS NULL OR MUSTERIIL = @Il) AND
		                                  (@Yil IS NULL OR DATEPART(YEAR,SIPARISTARIH) = @Yil)
                                )
                                SELECT
			                        Il 
			                        , CAST(ISNULL((SELECT SUM(CAST(ISNULL(ADET,'0') AS INT)) FROM KAPI_FILITRELE WHERE SIPARISTARIH >= @Period AND SIPARISTARIH < DATEADD(MONTH,1,@Period) AND Il=KF.Il),0) AS VARCHAR) AS '1'
			                        , CAST(ISNULL((SELECT SUM(CAST(ISNULL(ADET,'0') AS INT)) FROM KAPI_FILITRELE WHERE SIPARISTARIH >= DATEADD(MONTH,1,@Period) AND SIPARISTARIH < DATEADD(MONTH,2,@Period) AND Il=KF.Il),0) AS VARCHAR) AS '2'
			                        , CAST(ISNULL((SELECT SUM(CAST(ISNULL(ADET,'0') AS INT)) FROM KAPI_FILITRELE WHERE SIPARISTARIH >= DATEADD(MONTH,2,@Period) AND SIPARISTARIH < DATEADD(MONTH,3,@Period) AND Il=KF.Il),0) AS VARCHAR) AS '3'
			                        , CAST(ISNULL((SELECT SUM(CAST(ISNULL(ADET,'0') AS INT)) FROM KAPI_FILITRELE WHERE SIPARISTARIH >= DATEADD(MONTH,3,@Period) AND SIPARISTARIH < DATEADD(MONTH,4,@Period) AND Il=KF.Il),0) AS VARCHAR) AS '4'
			                        , CAST(ISNULL((SELECT SUM(CAST(ISNULL(ADET,'0') AS INT)) FROM KAPI_FILITRELE WHERE SIPARISTARIH >= DATEADD(MONTH,4,@Period) AND SIPARISTARIH < DATEADD(MONTH,5,@Period) AND Il=KF.Il),0) AS VARCHAR) AS '5'
			                        , CAST(ISNULL((SELECT SUM(CAST(ISNULL(ADET,'0') AS INT)) FROM KAPI_FILITRELE WHERE SIPARISTARIH >= DATEADD(MONTH,5,@Period) AND SIPARISTARIH < DATEADD(MONTH,6,@Period) AND Il=KF.Il),0) AS VARCHAR) AS '6'
			                        , CAST(ISNULL((SELECT SUM(CAST(ISNULL(ADET,'0') AS INT)) FROM KAPI_FILITRELE WHERE SIPARISTARIH >= DATEADD(MONTH,6,@Period) AND SIPARISTARIH < DATEADD(MONTH,7,@Period) AND Il=KF.Il),0) AS VARCHAR) AS '7'
			                        , CAST(ISNULL((SELECT SUM(CAST(ISNULL(ADET,'0') AS INT)) FROM KAPI_FILITRELE WHERE SIPARISTARIH >= DATEADD(MONTH,7,@Period) AND SIPARISTARIH < DATEADD(MONTH,8,@Period) AND Il=KF.Il),0) AS VARCHAR) AS '8'
			                        , CAST(ISNULL((SELECT SUM(CAST(ISNULL(ADET,'0') AS INT)) FROM KAPI_FILITRELE WHERE SIPARISTARIH >= DATEADD(MONTH,8,@Period) AND SIPARISTARIH < DATEADD(MONTH,9,@Period) AND Il=KF.Il),0) AS VARCHAR) AS '9'
			                        , CAST(ISNULL((SELECT SUM(CAST(ISNULL(ADET,'0') AS INT)) FROM KAPI_FILITRELE WHERE SIPARISTARIH >= DATEADD(MONTH,9,@Period) AND SIPARISTARIH < DATEADD(MONTH,10,@Period) AND Il=KF.Il),0) AS VARCHAR) AS '10'
			                        , CAST(ISNULL((SELECT SUM(CAST(ISNULL(ADET,'0') AS INT)) FROM KAPI_FILITRELE WHERE SIPARISTARIH >= DATEADD(MONTH,10,@Period) AND SIPARISTARIH < DATEADD(MONTH,11,@Period) AND Il=KF.Il),0) AS VARCHAR) AS '11'
			                        , CAST(ISNULL((SELECT SUM(CAST(ISNULL(ADET,'0') AS INT)) FROM KAPI_FILITRELE WHERE SIPARISTARIH >= DATEADD(MONTH,11,@Period) AND SIPARISTARIH < DATEADD(MONTH,12,@Period) AND Il=KF.Il),0) AS VARCHAR) AS '12'
                                    , CAST(ISNULL((SELECT SUM(CAST(ISNULL(ADET,'0') AS INT)) FROM KAPI_FILITRELE WHERE SIPARISTARIH >= @Period AND SIPARISTARIH < DATEADD(MONTH,12,@Period) AND Il=KF.Il),0) AS VARCHAR) AS 'Yillik'
                                    , '' AS [Yuzde(%)]
		                        FROM KAPI_FILITRELE AS KF
		                        GROUP BY Il";

            data.AddSqlParameter("Il", il, SqlDbType.VarChar, 50);
            data.AddSqlParameter("Yil", yil, SqlDbType.VarChar, 50);
            data.GetRecords(dt, sqlText);

            return dt;
        }

        private DataTable IlceSatilanAdetGetir(string il, string ilce, string yil)
        {
            DataTable dt = new DataTable();
            IData data = GetDataObject();
            string sqlText = @"DECLARE @Period AS DATE;
                                SET @Period = CAST('01.01.' + @Yil AS DATE);

                                WITH KAPI_FILITRELE AS 
                                (
	                                SELECT
		                                MUSTERIIL AS Il
		                                 , MUSTERIILCE AS IlIlce
		                                 , MUSTERISEMT AS Semt
		                                 , ADET
		                                 , CAST(SIPARISTARIH AS DATE) AS SIPARISTARIH
	                                FROM dbo.SIPARIS
	                                WHERE (@Il IS NULL OR MUSTERIIL = @Il) AND
		                                  (@Ilce IS NULL OR MUSTERIILCE = @Ilce) AND
		                                  (@Yil IS NULL OR DATEPART(YEAR,SIPARISTARIH) = @Yil)
                                )
                                SELECT
			                        IlIlce
			                        , CAST(ISNULL((SELECT SUM(CAST(ISNULL(ADET,'0') AS INT)) FROM KAPI_FILITRELE WHERE SIPARISTARIH >= @Period AND SIPARISTARIH < DATEADD(MONTH,1,@Period) AND IlIlce=KF.IlIlce),0) AS VARCHAR) AS '1'
			                        , CAST(ISNULL((SELECT SUM(CAST(ISNULL(ADET,'0') AS INT)) FROM KAPI_FILITRELE WHERE SIPARISTARIH >= DATEADD(MONTH,1,@Period) AND SIPARISTARIH < DATEADD(MONTH,2,@Period) AND IlIlce=KF.IlIlce),0) AS VARCHAR) AS '2'
			                        , CAST(ISNULL((SELECT SUM(CAST(ISNULL(ADET,'0') AS INT)) FROM KAPI_FILITRELE WHERE SIPARISTARIH >= DATEADD(MONTH,2,@Period) AND SIPARISTARIH < DATEADD(MONTH,3,@Period) AND IlIlce=KF.IlIlce),0) AS VARCHAR) AS '3'
			                        , CAST(ISNULL((SELECT SUM(CAST(ISNULL(ADET,'0') AS INT)) FROM KAPI_FILITRELE WHERE SIPARISTARIH >= DATEADD(MONTH,3,@Period) AND SIPARISTARIH < DATEADD(MONTH,4,@Period) AND IlIlce=KF.IlIlce),0) AS VARCHAR) AS '4'
			                        , CAST(ISNULL((SELECT SUM(CAST(ISNULL(ADET,'0') AS INT)) FROM KAPI_FILITRELE WHERE SIPARISTARIH >= DATEADD(MONTH,4,@Period) AND SIPARISTARIH < DATEADD(MONTH,5,@Period) AND IlIlce=KF.IlIlce),0) AS VARCHAR) AS '5'
			                        , CAST(ISNULL((SELECT SUM(CAST(ISNULL(ADET,'0') AS INT)) FROM KAPI_FILITRELE WHERE SIPARISTARIH >= DATEADD(MONTH,5,@Period) AND SIPARISTARIH < DATEADD(MONTH,6,@Period) AND IlIlce=KF.IlIlce),0) AS VARCHAR) AS '6'
			                        , CAST(ISNULL((SELECT SUM(CAST(ISNULL(ADET,'0') AS INT)) FROM KAPI_FILITRELE WHERE SIPARISTARIH >= DATEADD(MONTH,6,@Period) AND SIPARISTARIH < DATEADD(MONTH,7,@Period) AND IlIlce=KF.IlIlce),0) AS VARCHAR) AS '7'
			                        , CAST(ISNULL((SELECT SUM(CAST(ISNULL(ADET,'0') AS INT)) FROM KAPI_FILITRELE WHERE SIPARISTARIH >= DATEADD(MONTH,7,@Period) AND SIPARISTARIH < DATEADD(MONTH,8,@Period) AND IlIlce=KF.IlIlce),0) AS VARCHAR) AS '8'
			                        , CAST(ISNULL((SELECT SUM(CAST(ISNULL(ADET,'0') AS INT)) FROM KAPI_FILITRELE WHERE SIPARISTARIH >= DATEADD(MONTH,8,@Period) AND SIPARISTARIH < DATEADD(MONTH,9,@Period) AND IlIlce=KF.IlIlce),0) AS VARCHAR) AS '9'
			                        , CAST(ISNULL((SELECT SUM(CAST(ISNULL(ADET,'0') AS INT)) FROM KAPI_FILITRELE WHERE SIPARISTARIH >= DATEADD(MONTH,9,@Period) AND SIPARISTARIH < DATEADD(MONTH,10,@Period) AND IlIlce=KF.IlIlce),0) AS VARCHAR) AS '10'
			                        , CAST(ISNULL((SELECT SUM(CAST(ISNULL(ADET,'0') AS INT)) FROM KAPI_FILITRELE WHERE SIPARISTARIH >= DATEADD(MONTH,10,@Period) AND SIPARISTARIH < DATEADD(MONTH,11,@Period) AND IlIlce=KF.IlIlce),0) AS VARCHAR) AS '11'
			                        , CAST(ISNULL((SELECT SUM(CAST(ISNULL(ADET,'0') AS INT)) FROM KAPI_FILITRELE WHERE SIPARISTARIH >= DATEADD(MONTH,11,@Period) AND SIPARISTARIH < DATEADD(MONTH,12,@Period) AND IlIlce=KF.IlIlce),0) AS VARCHAR) AS '12'
		                            , CAST(ISNULL((SELECT SUM(CAST(ISNULL(ADET,'0') AS INT)) FROM KAPI_FILITRELE WHERE SIPARISTARIH >= @Period AND SIPARISTARIH < DATEADD(MONTH,12,@Period) AND IlIlce=KF.IlIlce),0) AS VARCHAR) AS 'Yillik'
                                    , '' AS [Yuzde(%)]
                                FROM KAPI_FILITRELE AS KF
		                        GROUP BY Il,IlIlce";

            data.AddSqlParameter("Il", il, SqlDbType.VarChar, 50);
            data.AddSqlParameter("Ilce", ilce, SqlDbType.VarChar, 50);
            data.AddSqlParameter("Yil", yil, SqlDbType.VarChar, 50);
            data.GetRecords(dt, sqlText);

            return dt;
        }

        private DataTable SemtSatilanAdetGetir(string il, string ilce, string yil)
        {
            DataTable dt = new DataTable();
            IData data = GetDataObject();
            string sqlText = @"DECLARE @Period AS DATE;
                                SET @Period = CAST('01.01.' + @Yil AS DATE);

                                WITH KAPI_FILITRELE AS 
                                (
	                                SELECT
		                                MUSTERIIL AS Il
		                                 , MUSTERIILCE AS IlIlce
		                                 , MUSTERISEMT AS Semt
		                                 , ADET
		                                 , CAST(SIPARISTARIH AS DATE) AS SIPARISTARIH
	                                FROM dbo.SIPARIS
	                                WHERE (@Il IS NULL OR MUSTERIIL = @Il) AND
		                                  (@Ilce IS NULL OR MUSTERIILCE = @Ilce) AND
		                                  (@Yil IS NULL OR DATEPART(YEAR,SIPARISTARIH) = @Yil)
                                )
                                SELECT
			                    Semt
			                    , CAST(ISNULL((SELECT SUM(CAST(ISNULL(ADET,'0') AS INT)) FROM KAPI_FILITRELE WHERE SIPARISTARIH >= @Period AND SIPARISTARIH < DATEADD(MONTH,1,@Period) AND Semt=KF.Semt),0) AS VARCHAR) AS '1'
			                    , CAST(ISNULL((SELECT SUM(CAST(ISNULL(ADET,'0') AS INT)) FROM KAPI_FILITRELE WHERE SIPARISTARIH >= DATEADD(MONTH,1,@Period) AND SIPARISTARIH < DATEADD(MONTH,2,@Period) AND Semt=KF.Semt),0) AS VARCHAR) AS '2'
			                    , CAST(ISNULL((SELECT SUM(CAST(ISNULL(ADET,'0') AS INT)) FROM KAPI_FILITRELE WHERE SIPARISTARIH >= DATEADD(MONTH,2,@Period) AND SIPARISTARIH < DATEADD(MONTH,3,@Period) AND Semt=KF.Semt),0) AS VARCHAR) AS '3'
			                    , CAST(ISNULL((SELECT SUM(CAST(ISNULL(ADET,'0') AS INT)) FROM KAPI_FILITRELE WHERE SIPARISTARIH >= DATEADD(MONTH,3,@Period) AND SIPARISTARIH < DATEADD(MONTH,4,@Period) AND Semt=KF.Semt),0) AS VARCHAR) AS '4'
			                    , CAST(ISNULL((SELECT SUM(CAST(ISNULL(ADET,'0') AS INT)) FROM KAPI_FILITRELE WHERE SIPARISTARIH >= DATEADD(MONTH,4,@Period) AND SIPARISTARIH < DATEADD(MONTH,5,@Period) AND Semt=KF.Semt),0) AS VARCHAR) AS '5'
			                    , CAST(ISNULL((SELECT SUM(CAST(ISNULL(ADET,'0') AS INT)) FROM KAPI_FILITRELE WHERE SIPARISTARIH >= DATEADD(MONTH,5,@Period) AND SIPARISTARIH < DATEADD(MONTH,6,@Period) AND Semt=KF.Semt),0) AS VARCHAR) AS '6'
			                    , CAST(ISNULL((SELECT SUM(CAST(ISNULL(ADET,'0') AS INT)) FROM KAPI_FILITRELE WHERE SIPARISTARIH >= DATEADD(MONTH,6,@Period) AND SIPARISTARIH < DATEADD(MONTH,7,@Period) AND Semt=KF.Semt),0) AS VARCHAR) AS '7'
			                    , CAST(ISNULL((SELECT SUM(CAST(ISNULL(ADET,'0') AS INT)) FROM KAPI_FILITRELE WHERE SIPARISTARIH >= DATEADD(MONTH,7,@Period) AND SIPARISTARIH < DATEADD(MONTH,8,@Period) AND Semt=KF.Semt),0) AS VARCHAR) AS '8'
			                    , CAST(ISNULL((SELECT SUM(CAST(ISNULL(ADET,'0') AS INT)) FROM KAPI_FILITRELE WHERE SIPARISTARIH >= DATEADD(MONTH,8,@Period) AND SIPARISTARIH < DATEADD(MONTH,9,@Period) AND Semt=KF.Semt),0) AS VARCHAR) AS '9'
			                    , CAST(ISNULL((SELECT SUM(CAST(ISNULL(ADET,'0') AS INT)) FROM KAPI_FILITRELE WHERE SIPARISTARIH >= DATEADD(MONTH,9,@Period) AND SIPARISTARIH < DATEADD(MONTH,10,@Period) AND Semt=KF.Semt),0) AS VARCHAR) AS '10'
			                    , CAST(ISNULL((SELECT SUM(CAST(ISNULL(ADET,'0') AS INT)) FROM KAPI_FILITRELE WHERE SIPARISTARIH >= DATEADD(MONTH,10,@Period) AND SIPARISTARIH < DATEADD(MONTH,11,@Period) AND Semt=KF.Semt),0) AS VARCHAR) AS '11'
			                    , CAST(ISNULL((SELECT SUM(CAST(ISNULL(ADET,'0') AS INT)) FROM KAPI_FILITRELE WHERE SIPARISTARIH >= DATEADD(MONTH,11,@Period) AND SIPARISTARIH < DATEADD(MONTH,12,@Period) AND Semt=KF.Semt),0) AS VARCHAR) AS '12'
                                , CAST(ISNULL((SELECT SUM(CAST(ISNULL(ADET,'0') AS INT)) FROM KAPI_FILITRELE WHERE SIPARISTARIH >= @Period AND SIPARISTARIH < DATEADD(MONTH,12,@Period) AND Semt=KF.Semt),0) AS VARCHAR) AS 'Yillik' 
                                , '' AS [Yuzde(%)]
		                    FROM KAPI_FILITRELE AS KF
		                    GROUP BY Il,IlIlce,Semt";

            data.AddSqlParameter("Il", il, SqlDbType.VarChar, 50);
            data.AddSqlParameter("Ilce", ilce, SqlDbType.VarChar, 50);
            data.AddSqlParameter("Yil", yil, SqlDbType.VarChar, 50);
            data.GetRecords(dt, sqlText);

            return dt;
        }
    }
}
