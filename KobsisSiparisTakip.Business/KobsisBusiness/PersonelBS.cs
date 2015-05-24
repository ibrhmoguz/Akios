﻿using System.Collections.Generic;
using System.Data;
using WebFrame.Business;
using WebFrame.DataAccess;
using WebFrame.DataType.Common.Attributes;
using System;
using WebFrame.DataType.Common.Logging;

namespace KobsisSiparisTakip.Business
{
    [ServiceConnectionNameAttribute("KobsisConnectionString")]
    public class PersonelBS : BusinessBase
    {
        public DataTable PersonelListesiGetir(int musteriID)
        {
            DataTable dt = new DataTable();
            dt.TableName = "PERSONEL";
            IData data = GetDataObject();
            data.AddSqlParameter("MusteriID", musteriID, SqlDbType.Int, 50);
            string sqlText = @"SELECT ID, RTRIM(Ad)+' ' + Soyad AS AD FROM PERSONEL WHERE MusteriID=@MusteriID ORDER BY 1";
            data.GetRecords(dt, sqlText);
            return dt;
        }

        public bool PersonelTanimla(int musteriID, string ad, string soyad)
        {
            IData data = GetDataObject();

            data.AddSqlParameter("MusteriID", musteriID, SqlDbType.Int, 50);
            data.AddSqlParameter("Ad", ad, SqlDbType.VarChar, 50);
            data.AddSqlParameter("Soyad", soyad, SqlDbType.VarChar, 50);

            string sqlKaydet = @"INSERT INTO PERSONEL (MusteriID,Ad,Soyad) VALUES (@MusteriID,@Ad,@Soyad)";
            data.ExecuteStatement(sqlKaydet);

            return true;
        }

        public bool PersonelSil(int id)
        {
            IData data = GetDataObject();
            data.AddSqlParameter("ID", id, SqlDbType.VarChar, 50);

            string sqlSil = @"DELETE FROM PERSONEL WHERE ID=@ID";
            data.ExecuteStatement(sqlSil);

            return true;
        }
    }
}
