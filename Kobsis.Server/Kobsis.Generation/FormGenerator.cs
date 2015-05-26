﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Kobsis.Business;
using Kobsis.DataType;
using Kobsis.Util;
using Telerik.Web.UI;

namespace Kobsis.Generation
{
    public class FormGenerator
    {
        public string SiparisSeri { get; set; }

        public FormIslemTipi IslemTipi { get; set; }

        public WebControl Generate(WebControl wc)
        {
            if (!SessionManager.KullaniciBilgi.MusteriID.HasValue) return wc;

            string parentKontrolID = string.Empty;
            List<Layout> seriLayoutList = null;
            if (this.IslemTipi == FormIslemTipi.Sorgula)
            {
                if (SessionManager.SiparisSorgulaFormLayout == null)
                {
                    SessionManager.SiparisSorgulaFormLayout = new FormLayoutBS().SorgulamaFormKontrolleriniGetir(SessionManager.KullaniciBilgi.MusteriID.Value);
                }

                if (SessionManager.SiparisSorgulaFormLayout != null)
                {
                    seriLayoutList = SessionManager.SiparisSorgulaFormLayout.Where(q => q.SeriID == Convert.ToInt32(this.SiparisSeri)).ToList();
                }
            }
            else
            {
                if (SessionManager.SiparisFormLayout == null)
                {
                    SessionManager.SiparisFormLayout = new FormLayoutBS().FormKontrolleriniGetir(SessionManager.KullaniciBilgi.MusteriID.Value);
                }

                if (SessionManager.SiparisFormLayout != null)
                {
                    seriLayoutList = SessionManager.SiparisFormLayout.Where(q => q.SeriID == Convert.ToInt32(this.SiparisSeri)).ToList();
                }
            }

            if (seriLayoutList != null)
            {
                foreach (Layout layout in seriLayoutList)
                {
                    WebControl wcTemp = KontrolOlustur(layout);
                    if (wcTemp == null)
                        continue;

                    if (layout.YerlesimParentID == null)
                    {
                        wc.Controls.Add(wcTemp);
                    }
                    else
                    {
                        parentKontrolID = ParentKontrolIDBul(seriLayoutList, layout.YerlesimParentID.Value);
                        WebControl wcParent = KontrolBul(wc, parentKontrolID);
                        if (wcParent != null)
                            wcParent.Controls.Add(wcTemp);
                    }
                }
            }
            return wc;
        }

        public object KontrolDegeriBul(Panel divPanel, string kontrolAdi, string yerlesimTabloID)
        {
            object kontrolDegeri = null;
            var control = KontrolBul(divPanel, kontrolAdi.Replace(" ", string.Empty) + yerlesimTabloID);

            if (control is RadTextBox)
            {
                kontrolDegeri = !string.IsNullOrWhiteSpace(((RadTextBox)control).Text) ? ((RadTextBox)control).Text : null;
            }
            if (control is RadMaskedTextBox)
            {
                kontrolDegeri = !string.IsNullOrWhiteSpace(((RadMaskedTextBox)control).Text) ? ((RadMaskedTextBox)control).Text : null;
            }
            else if (control is RadNumericTextBox)
            {
                kontrolDegeri = !string.IsNullOrWhiteSpace(((RadNumericTextBox)control).Text) ? ((RadNumericTextBox)control).Text : null;
            }
            else if (control is CheckBox)
            {
                kontrolDegeri = ((CheckBox)control).Checked == true ? ((CheckBox)control).Checked : false;
            }
            else if (control is RadDatePicker)
            {
                kontrolDegeri = ((RadDatePicker)control).SelectedDate != null ? ((RadDatePicker)control).SelectedDate : null;
            }
            else if (control is RadDropDownList)
            {
                kontrolDegeri = ((RadDropDownList)control).SelectedValue != null && ((RadDropDownList)control).SelectedValue != "0" ? ((RadDropDownList)control).SelectedValue : null;
            }
            else if (control is Label)
            {
                kontrolDegeri = !string.IsNullOrWhiteSpace(((Label)control).Text) ? ((Label)control).Text : null;
            }

            return kontrolDegeri;
        }

        public SqlDbType VeriTipiBelirle(string veriTipAdi)
        {
            var sqlType = new SqlDbType();

            switch (veriTipAdi)
            {
                case VeriTipi.BOOLEAN:
                    sqlType = SqlDbType.Bit;
                    break;
                case VeriTipi.DATETIME:
                    sqlType = SqlDbType.DateTime;
                    break;
                case VeriTipi.DATE:
                    sqlType = SqlDbType.Date;
                    break;
                case VeriTipi.DECIMAL:
                    sqlType = SqlDbType.Decimal;
                    break;
                case VeriTipi.INTEGER:
                    sqlType = SqlDbType.Int;
                    break;
                case VeriTipi.STRING:
                    sqlType = SqlDbType.VarChar;
                    break;
            }
            return sqlType;
        }

        private string ParentKontrolIDBul(List<Layout> layoutList, int parentID)
        {
            string parentKontrolID = string.Empty;
            foreach (Layout layout in layoutList)
            {
                if (layout.YerlesimID == parentID)
                {
                    parentKontrolID = KontrolIDGetir(layout);
                    break;
                }
            }
            return parentKontrolID;
        }

        private WebControl KontrolBul(WebControl rootControl, string controlID)
        {
            if (rootControl.ID == controlID) return rootControl;

            foreach (WebControl controlToSearch in rootControl.Controls)
            {
                WebControl controlToReturn = KontrolBul(controlToSearch, controlID);
                if (controlToReturn != null) return controlToReturn;
            }
            return null;
        }

        private WebControl KontrolOlustur(Layout layout)
        {
            WebControl wc = null;

            switch ((KontrolTipEnum)Enum.Parse(typeof(KontrolTipEnum), layout.KontrolTipID.ToString()))
            {
                case KontrolTipEnum.Table:
                    wc = TableOlustur(layout);
                    break;
                case KontrolTipEnum.TableRow:
                    wc = TableRowOlustur(layout);
                    break;
                case KontrolTipEnum.TableCell:
                    wc = TableCellOlustur(layout);
                    break;
                case KontrolTipEnum.TableHeaderCell:
                    wc = TableHeaderCellOlustur(layout);
                    break;
                case KontrolTipEnum.TextBox:
                    wc = this.IslemTipi == FormIslemTipi.Goruntule ? LabelOlustur(layout) : TextBoxOlustur(layout);
                    break;
                case KontrolTipEnum.NumericTextBox:
                    wc = this.IslemTipi == FormIslemTipi.Goruntule ? LabelOlustur(layout) : NumericTextBoxOlustur(layout);
                    break;
                case KontrolTipEnum.MaskedTextBox:
                    wc = this.IslemTipi == FormIslemTipi.Goruntule ? LabelOlustur(layout) : MaskedTextBoxOlustur(layout);
                    break;
                case KontrolTipEnum.Literal:
                    wc = LiteralOlustur(layout);
                    break;
                case KontrolTipEnum.Label:
                    wc = LabelOlustur(layout);
                    break;
                case KontrolTipEnum.Image:
                    wc = ImageOlustur(layout);
                    break;
                case KontrolTipEnum.DropDownList:
                    wc = this.IslemTipi == FormIslemTipi.Goruntule ? LabelOlustur(layout) : DropDownListOlustur(layout);
                    break;
                case KontrolTipEnum.DatePicker:
                    wc = this.IslemTipi == FormIslemTipi.Goruntule ? LabelOlustur(layout) : DateTimePickerOlustur(layout);
                    break;
                case KontrolTipEnum.CheckBox:
                    wc = CheckBoxOlustur(layout);
                    break;
            }

            return wc;
        }

        private WebControl CheckBoxOlustur(Layout layout)
        {
            var wc = new CheckBox();
            if (layout.PostBack) wc.AutoPostBack = true;
            if (!string.IsNullOrWhiteSpace(layout.Text)) wc.Text = layout.Text;
            KontrolOzellikAyarla(layout, wc);
            if (this.IslemTipi == FormIslemTipi.Guncelle && !string.IsNullOrWhiteSpace(layout.KolonAdi))
            {
                if (SessionManager.SiparisBilgi != null && SessionManager.SiparisBilgi.Rows.Count > 0 && SessionManager.SiparisBilgi.Rows[0][layout.KolonAdi] != DBNull.Value)
                {
                    bool value;
                    if (Boolean.TryParse(SessionManager.SiparisBilgi.Rows[0][layout.KolonAdi].ToString(), out value))
                        wc.Checked = value;
                }
            }
            return wc;
        }

        private WebControl DateTimePickerOlustur(Layout layout)
        {
            var wc = new RadDatePicker();
            if (layout.PostBack) wc.AutoPostBack = true;
            if (!string.IsNullOrWhiteSpace(layout.Text)) wc.SelectedDate = Convert.ToDateTime(layout.Text);
            KontrolOzellikAyarla(layout, wc);
            if (this.IslemTipi == FormIslemTipi.Guncelle && !string.IsNullOrWhiteSpace(layout.KolonAdi))
            {
                if (SessionManager.SiparisBilgi != null && SessionManager.SiparisBilgi.Rows.Count > 0 && SessionManager.SiparisBilgi.Rows[0][layout.KolonAdi] != DBNull.Value)
                {
                    DateTime value;
                    if (DateTime.TryParse(SessionManager.SiparisBilgi.Rows[0][layout.KolonAdi].ToString(), out value))
                        wc.SelectedDate = value;
                }
            }
            return wc;
        }

        private WebControl DropDownListOlustur(Layout layout)
        {
            var wc = new RadDropDownList();
            wc.RenderMode = RenderMode.Lightweight;
            wc.DataValueField = "RefDetayID";
            wc.DataTextField = "RefDetayAdi";
            if (layout.PostBack) wc.AutoPostBack = true;
            KontrolOzellikAyarla(layout, wc);
            if (layout.RefID.HasValue)
            {
                wc.DataSource = new ReferansDataBS() { SiparisSeri = this.SiparisSeri, RefID = layout.RefID.Value.ToString() }.ReferansVerisiGetir();
                wc.DataBind();
                if (wc.Items != null)
                    wc.Items.Insert(0, new Telerik.Web.UI.DropDownListItem("Seçiniz", "0"));
            }
            if (this.IslemTipi == FormIslemTipi.Guncelle && !string.IsNullOrWhiteSpace(layout.KolonAdi))
            {
                if (SessionManager.SiparisBilgi != null && SessionManager.SiparisBilgi.Rows.Count > 0 && SessionManager.SiparisBilgi.Rows[0][layout.KolonAdi] != DBNull.Value)
                {
                    DropDownListItem ddli = wc.FindItemByValue(SessionManager.SiparisBilgi.Rows[0][layout.KolonAdi].ToString());
                    if (ddli != null && ddli.Selected == false)
                        ddli.Selected = true;
                }
            }
            return wc;
        }

        private WebControl ImageOlustur(Layout layout)
        {
            var wc = new Image();
            wc.ImageUrl = "ImageForm.aspx?ImageID=" + layout.ImajID;
            KontrolOzellikAyarla(layout, wc);
            return wc;
        }

        private WebControl LabelOlustur(Layout layout)
        {
            var wc = new Label();
            if (!string.IsNullOrWhiteSpace(layout.Text)) wc.Text = layout.Text;
            KontrolOzellikAyarla(layout, wc);
            if (this.IslemTipi != FormIslemTipi.Kaydet && !string.IsNullOrWhiteSpace(layout.KolonAdi))
            {
                if (layout.RefID.HasValue)
                {
                    DataTable dt = new ReferansDataBS() { SiparisSeri = this.SiparisSeri, RefID = layout.RefID.Value.ToString() }.ReferansVerisiGetir();
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        if (SessionManager.SiparisBilgi != null && SessionManager.SiparisBilgi.Rows.Count > 0 && SessionManager.SiparisBilgi.Rows[0][layout.KolonAdi] != DBNull.Value)
                        {
                            DataRow dr = dt.AsEnumerable().SingleOrDefault(p => p.Field<int>("RefDetayID") == Convert.ToInt32(SessionManager.SiparisBilgi.Rows[0][layout.KolonAdi].ToString()));
                            wc.Text = (dr != null && dr["RefDetayAdi"] != DBNull.Value) ? dr["RefDetayAdi"].ToString() : string.Empty;
                        }
                    }
                }
                else if (SessionManager.SiparisBilgi != null && SessionManager.SiparisBilgi.Rows.Count > 0 && SessionManager.SiparisBilgi.Rows[0][layout.KolonAdi] != DBNull.Value)
                {
                    wc.Text = SessionManager.SiparisBilgi.Rows[0][layout.KolonAdi].ToString();
                }
            }
            return wc;
        }

        private WebControl LiteralOlustur(Layout layout)
        {
            if (!string.IsNullOrWhiteSpace(layout.Text))
            {
                return new LiteralWebControl(layout.Text) { ID = KontrolIDGetir(layout) };
            }
            else
                return null;
        }

        private WebControl TextBoxOlustur(Layout layout)
        {
            var wc = new RadTextBox();
            wc.RenderMode = RenderMode.Lightweight;
            if (!string.IsNullOrWhiteSpace(layout.TextMode)) wc.TextMode = (InputMode)Enum.Parse(typeof(InputMode), layout.TextMode);
            if (!string.IsNullOrWhiteSpace(layout.Text)) wc.Text = layout.Text;
            KontrolOzellikAyarla(layout, wc);
            if (this.IslemTipi == FormIslemTipi.Guncelle && !string.IsNullOrWhiteSpace(layout.KolonAdi))
            {
                if (SessionManager.SiparisBilgi != null && SessionManager.SiparisBilgi.Rows.Count > 0 && SessionManager.SiparisBilgi.Rows[0][layout.KolonAdi] != DBNull.Value)
                {
                    wc.Text = SessionManager.SiparisBilgi.Rows[0][layout.KolonAdi].ToString();
                }
            }
            return wc;
        }

        private WebControl MaskedTextBoxOlustur(Layout layout)
        {
            var wc = new RadMaskedTextBox();
            wc.RenderMode = RenderMode.Lightweight;
            if (!string.IsNullOrWhiteSpace(layout.Mask)) wc.Mask = layout.Mask;
            if (!string.IsNullOrWhiteSpace(layout.Text)) wc.Text = layout.Text;
            KontrolOzellikAyarla(layout, wc);
            if (this.IslemTipi == FormIslemTipi.Guncelle && !string.IsNullOrWhiteSpace(layout.KolonAdi))
            {
                if (SessionManager.SiparisBilgi != null && SessionManager.SiparisBilgi.Rows.Count > 0 && SessionManager.SiparisBilgi.Rows[0][layout.KolonAdi] != DBNull.Value)
                {
                    wc.Text = SessionManager.SiparisBilgi.Rows[0][layout.KolonAdi].ToString();
                }
            }
            return wc;
        }

        private WebControl NumericTextBoxOlustur(Layout layout)
        {
            var wc = new RadNumericTextBox();
            wc.RenderMode = RenderMode.Lightweight;
            wc.CssClass = "NumericFieldClass";
            if (!string.IsNullOrWhiteSpace(layout.Text)) wc.Text = layout.Text;
            KontrolOzellikAyarla(layout, wc);
            if (this.IslemTipi == FormIslemTipi.Guncelle && !string.IsNullOrWhiteSpace(layout.KolonAdi))
            {
                if (SessionManager.SiparisBilgi != null && SessionManager.SiparisBilgi.Rows.Count > 0 && SessionManager.SiparisBilgi.Rows[0][layout.KolonAdi] != DBNull.Value)
                {
                    wc.Text = SessionManager.SiparisBilgi.Rows[0][layout.KolonAdi].ToString();
                }
            }
            return wc;
        }

        private WebControl TableHeaderCellOlustur(Layout layout)
        {
            var wc = new TableHeaderCell();
            if (!string.IsNullOrWhiteSpace(layout.Text)) wc.Text = layout.Text;
            KontrolOzellikAyarla(layout, wc);
            return wc;
        }

        private WebControl TableCellOlustur(Layout layout)
        {
            var wc = new TableCell();
            if (!string.IsNullOrWhiteSpace(layout.Text)) wc.Text = layout.Text;
            KontrolOzellikAyarla(layout, wc);
            return wc;
        }

        private WebControl TableRowOlustur(Layout layout)
        {
            return new TableRow() { ID = KontrolIDGetir(layout) };
        }

        private WebControl TableOlustur(Layout layout)
        {
            var wc = new Table();
            KontrolOzellikAyarla(layout, wc);
            return wc;
        }

        private void KontrolOzellikAyarla(Layout layout, WebControl wc)
        {
            wc.ID = KontrolIDGetir(layout);
            wc.Enabled = layout.Enabled;
            if (layout.Yukseklik != null) wc.Height = new Unit(layout.Yukseklik.Value);
            if (layout.Genislik != null) wc.Width = new Unit(layout.Genislik.Value);
            if (!String.IsNullOrWhiteSpace(layout.CssClass)) wc.CssClass = layout.CssClass;
            if (!String.IsNullOrWhiteSpace(layout.Style)) wc.Attributes.Add("style", layout.Style);
            if (layout.RowSpan != null) wc.Attributes.Add("rowspan", layout.RowSpan.ToString());
            if (layout.ColSpan != null) wc.Attributes.Add("colspan", layout.ColSpan.ToString());
        }

        private string KontrolIDGetir(Layout layout)
        {
            return layout.KontrolAdi.Replace(" ", string.Empty) + layout.YerlesimTabloID.ToString();
        }
    }
}