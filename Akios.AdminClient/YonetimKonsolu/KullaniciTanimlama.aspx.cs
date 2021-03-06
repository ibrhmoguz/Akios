﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI.WebControls;
using Akios.AdminWebClient.Helper;
using Akios.Business;
using Akios.Util;
using Telerik.Web.UI;

namespace Akios.AdminWebClient.YonetimKonsolu
{
    public partial class KullaniciTanimlama : AkiosBasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                RolleriDoldur();
            }
            
            KullaniciDoldur();
        }

        private void RolleriDoldur()
        {
            DataTable dt = new ReferansDataBS().KullaniciRolleriGetir();

            if (dt.Rows.Count > 0)
            {
                ddlKullaniciRol.DataSource = dt;
                ddlKullaniciRol.DataTextField = "RolAdi";
                ddlKullaniciRol.DataValueField = "RolID";
                ddlKullaniciRol.DataBind();
                ddlKullaniciRol.Items.Insert(0, new Telerik.Web.UI.DropDownListItem("Seçiniz", "0"));
            }
            else
            {
                ddlKullaniciRol.DataSource = null;
                ddlKullaniciRol.DataBind();
            }
        }

        private void KullaniciDoldur()
        {
            string musteriId = MusteriGetir();
            if (String.IsNullOrWhiteSpace(musteriId) || musteriId.Equals("0"))
            {
                RP_Kullanici.DataSource = new KullaniciBS().TumKullanicilariGetir();
                RP_Kullanici.DataBind();
            }
            else
            {
                RP_Kullanici.DataSource = new KullaniciBS().KullanicilariGetir(Convert.ToInt32(musteriId));
                RP_Kullanici.DataBind();
            }
        }

        private string MusteriGetir()
        {
            if (this.Master != null)
            {
                var rddlMusteri = (RadDropDownList)this.Master.FindControl("ddlMusteri");
                return rddlMusteri.SelectedValue;
            }
            return string.Empty;
        }

        protected void btnEkle_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(txtKullaniciAdi.Text))
            {
                MessageBox.Uyari(this, "Kullanıcı adı giriniz");
                return;
            }
            if (ddlKullaniciRol.SelectedValue == null || ddlKullaniciRol.SelectedValue.Equals("") || ddlKullaniciRol.SelectedValue.Equals("0"))
            {
                MessageBox.Uyari(this, "Kullanıcı rolü seçiniz");
                return;
            }
            if (String.IsNullOrWhiteSpace(MusteriGetir()) || MusteriGetir().Equals("0"))
            {
                MessageBox.Uyari(this, "Müşteri seçiniz");
                return;
            }

            string kullanici = txtKullaniciAdi.Text.Trim();
            string yetki = ddlKullaniciRol.SelectedValue;
            string musteri = MusteriGetir();
            string sifre = "12345";
            bool sonuc = false;

            Dictionary<string, object> prms = new Dictionary<string, object>();
            prms.Add("KullaniciAdi", kullanici);
            prms.Add("RolID", yetki);
            prms.Add("Sifre", sifre);
            prms.Add("MusteriID", musteri);

            sonuc = new KullaniciBS().KullaniciTanimla(prms);

            if (sonuc)
            {
                KullaniciDoldur();
                MessageBox.Basari(this, "Kullanici eklendi.");
            }
            else
            {
                MessageBox.Hata(this, "Kullanıcı ekleme işleminde hata oluştu!");
            }
        }

        protected void RP_Kullanici_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            bool sonuc = false;

            if (e.CommandName == "Delete" && e.CommandArgument != null && !string.IsNullOrEmpty(e.CommandArgument.ToString()))
            {
                sonuc = new KullaniciBS().KullaniciSil(Convert.ToInt32(e.CommandArgument.ToString()));

                if (sonuc)
                {
                    KullaniciDoldur();
                    MessageBox.Basari(this, "Kullanıcı silindi.");
                }
                else
                {
                    MessageBox.Hata(this, "Kullanıcı silme işleminde hata oluştu!");
                }
            }
        }
    }
}