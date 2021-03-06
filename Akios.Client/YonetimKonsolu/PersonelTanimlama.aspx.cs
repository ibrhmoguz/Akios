﻿using System;
using System.Data;
using System.Web.UI.WebControls;
using Akios.Business;
using Akios.DataType;
using Akios.Util;
using Akios.WebClient.Helper;

namespace Akios.WebClient.YonetimKonsolu
{
    public partial class PersonelTanimlama : AkiosBasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (SessionManager.KullaniciBilgi.Rol == KullaniciRol.Kullanici)
            {
                MessageBox.Hata(this, "Bu sayfaya erişim yetkiniz yoktur!");
                return;
            }

            if (!Page.IsPostBack)
            {
                PersonelDoldur();
            }
        }

        private void PersonelDoldur()
        {
            if (SessionManager.MusteriBilgi.MusteriID != null)
            {
                DataTable dt = new PersonelBS().PersonelListesiGetir(SessionManager.MusteriBilgi.MusteriID.Value);
                RP_Personel.DataSource = dt;
                RP_Personel.DataBind();
                SessionManager.PersonelListesi = dt;
            }
        }

        protected void btnEkle_Click(object sender, EventArgs e)
        {
            string ad = txtAd.Text.Trim();
            string soyad = txtSoyad.Text.Trim();

            bool sonuc = false;

            if (SessionManager.MusteriBilgi.MusteriID != null)
                sonuc = new PersonelBS().PersonelTanimla(SessionManager.MusteriBilgi.MusteriID.Value.ToString(), ad, soyad);

            if (sonuc)
            {
                PersonelDoldur();
                MessageBox.Basari(this, "Personel eklendi.");
            }
            else
            {
                MessageBox.Hata(this, "Personel ekleme işleminde hata oluştu!");
            }
        }

        protected void RP_Personel_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            bool sonuc = false;

            if (e.CommandName == "Delete")
            {
                string id = e.CommandArgument.ToString();
                if (string.IsNullOrWhiteSpace(id)) return;

                sonuc = new PersonelBS().PersonelSil(Convert.ToInt32(id));

                if (sonuc)
                {
                    PersonelDoldur();
                    MessageBox.Basari(this, "Personel silindi.");
                }
                else
                {
                    MessageBox.Hata(this, "Personel silme işleminde hata oluştu!");
                }
            }
        }
    }
}