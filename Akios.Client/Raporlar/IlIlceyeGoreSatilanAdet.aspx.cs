﻿using System;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web.UI.WebControls;
using Akios.Business;
using Akios.Util;
using Akios.WebClient.Helper;

namespace Akios.WebClient.Raporlar
{
    public partial class IlIlceyeGoreSatilanAdet : AkiosBasePage
    {
        private static string ANKARA_IL_KODU = "6";
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                VarsayilanDegerleriYukle();
            }
        }

        private void VarsayilanDegerleriYukle()
        {
            IlleriGetir();
            YillariYukle();
        }

        private void YillariYukle()
        {
            ddlYil.Items.Clear();
            for (int i = 2014; i < 2030; i++)
            {
                Telerik.Web.UI.DropDownListItem item = new Telerik.Web.UI.DropDownListItem(i.ToString(), i.ToString());
                if (item.Value == DateTime.Now.Year.ToString())
                    item.Selected = true;
                ddlYil.Items.Add(item);
            }
        }

        protected void ddlMusteriIl_SelectedIndexChanged(object sender, Telerik.Web.UI.DropDownListEventArgs e)
        {
            ddlMusteriIlce.Items.Clear();
            IlceleriGetir(e.Value);
        }

        private void IlleriGetir()
        {
            DataTable dt = new ReferansDataBS().IlleriGetir();
            if (dt.Rows.Count > 0)
            {
                ddlMusteriIl.DataSource = dt;
                ddlMusteriIl.DataTextField = "ILAD";
                ddlMusteriIl.DataValueField = "ILKOD";
                ddlMusteriIl.DataBind();

                ddlMusteriIl.FindItemByValue(ANKARA_IL_KODU).Selected = true;
                IlceleriGetir(ANKARA_IL_KODU);
            }
            else
            {
                ddlMusteriIl.DataSource = null;
                ddlMusteriIl.DataBind();
            }
            ddlMusteriIl.Items.Insert(0, new Telerik.Web.UI.DropDownListItem("Seçiniz", "0"));
        }

        private void IlceleriGetir(string ilKod)
        {
            DataTable dt = new ReferansDataBS().IlceleriGetir(ilKod);
            if (dt.Rows.Count > 0)
            {
                ddlMusteriIlce.DataSource = dt;
                ddlMusteriIlce.DataTextField = "ILCEAD";
                ddlMusteriIlce.DataValueField = "ILCEKOD";
                ddlMusteriIlce.DataBind();
            }
            else
            {
                ddlMusteriIlce.DataSource = null;
                ddlMusteriIlce.DataBind();
            }
            ddlMusteriIlce.Items.Insert(0, new Telerik.Web.UI.DropDownListItem("Seçiniz", "0"));
            ddlMusteriIlce.SelectedIndex = 0;
        }

        protected void btnSorgula_Click(object sender, EventArgs e)
        {
            string il = null;
            string ilce = null;

            if (ddlMusteriIl.SelectedIndex != 0)
                il = ddlMusteriIl.SelectedValue;
            if (ddlMusteriIlce.SelectedIndex != 0)
                ilce = ddlMusteriIlce.SelectedValue;

            DataSet ds = new RaporBS().IlIlceyeGoreSatilanAdet(il, ilce, ddlYil.SelectedValue, SessionManager.MusteriBilgi.Kod);

            DataTable dt1 = YuzdeDegerleriHesapla(ds.Tables[0]);
            DataTable dt2 = YuzdeDegerleriHesapla(ds.Tables[1]);
            DataTable dt3 = YuzdeDegerleriHesapla(ds.Tables[2]);

            GridDoldur(grdRaporIl, dt1);
            GridDoldur(grdRaporIlce, dt2);
            GridDoldur(grdRaporSemt, dt3);

            SessionManager.IlIlceyeGoreSatilanAdet = ds;
            PopupPageHelper.OpenPopUp(btnYazdir, "../Print/IlIlceyeGoreSatilanAdet.aspx", "", true, false, true, false, false, false, 1024, 800, true, false, "onclick");
            btnYazdir.Visible = true;
        }

        private void GridDoldur(GridView gv, DataTable dt)
        {
            if (dt != null && dt.Rows.Count > 0)
            {
                gv.DataSource = dt;
                gv.DataBind();
            }
            else
            {
                gv.DataSource = null;
                gv.DataBind();
            }
        }

        private DataTable YuzdeDegerleriHesapla(DataTable dt)
        {
            decimal toplamAdet = Convert.ToDecimal(dt.AsEnumerable().Sum(a => Convert.ToInt32(a.Field<string>("Yillik"))).ToString());
            decimal yuzde;

            if (toplamAdet != 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    if (row["Yillik"] != DBNull.Value)
                    {
                        yuzde = Convert.ToDecimal((Convert.ToDecimal(row["Yillik"].ToString())/toplamAdet));
                        row["Yuzde(%)"] = (yuzde*100).ToString("0.00", CultureInfo.InvariantCulture);
                    }
                }
            }

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    if (dt.Rows[i][j] != DBNull.Value && dt.Rows[i][j].ToString() == "0")
                        dt.Rows[i][j] = string.Empty;
                }
            }

            return dt;
        }
    }
}