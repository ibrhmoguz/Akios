﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Print/PrinterFriendly.Master" AutoEventWireup="true" CodeBehind="PrintYangin.aspx.cs" Inherits="Akios.AdminWebClient.Print.PrintYangin" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder2" runat="server">

    <table class="normalTablo" style="width: 100%">
        <tr>
            <td rowspan="6" style="text-align: center">
                <telerik:RadBinaryImage ID="imgLogo" runat="server" ImageUrl="~/App_Themes/Theme/Raster/ackLogo.PNG" Width="80" Height="80" />
            </td>
            <td colspan="2" rowspan="3" style="width: 45%; font-size: x-large; text-align: center;">

                <b>
                    <asp:Label ID="lblKapiTur" runat="server"></asp:Label></b>
            </td>
            <td style="width: 30%; text-align: left">
                <b>ANKARA ÇELİK KAPI SAN. TİC. LTD. ŞTİ. </b>
            </td>
        </tr>
        <tr>

            <td style="font-size: smaller; text-align: left">
                <b>Adres: </b>Alınteri Bulvarı No:212 Ostim/ANKARA
            </td>
        </tr>
        <tr>
            <td style="font-size: smaller; text-align: left">
                <b>Telefon: </b>(0 312) 385 37 83 - 84
            </td>

        </tr>
        <tr>
            <td colspan="2" rowspan="3" style="font-size: x-large; text-align: center">
                <b>SİPARİŞ FORMU</b>
            </td>
            <td style="font-size: smaller; text-align: left">
                <b>Faks : </b>(0 312) 354 61 81
            </td>
        </tr>
        <tr>
            <td style="font-size: smaller; text-align: left">
                <b>Web : </b>www.ankaracelikkapi.com.tr
            </td>
        </tr>
        <tr>
            <td style="font-size: smaller; text-align: left">
                <b>e-posta : </b>ankara@celikkapi.net
            </td>
        </tr>
    </table>
    <table class="boldTablo" style="width: 100%">
        <tr>
            <td style="width: 15%">Ölçü Tarihi:   </td>
            <td style="width: 35%">
                <asp:Label ID="lblOlcuTarihSaat" runat="server"></asp:Label>
            </td>
            <td style="width: 12%">Sipariş Tarihi :  </td>
            <td>
                <asp:Label ID="lblSiparisTarih" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>Bayi Adı : </td>
            <td>
                <asp:Label ID="lblBayiAdi" runat="server"></asp:Label>
            </td>
            <td>Sipariş No : </td>
            <td>
                <asp:Label ID="lblSiparisNo" runat="server"></asp:Label>
            </td>
        </tr>
        <%-- <tr>
            <td>Sipariş Durumu : </td>
            <td>
                <asp:Label ID="lblSiparisDurum" runat="server"></asp:Label>
            </td>
            <td colspan="2"></td>
        </tr>--%>
    </table>
    <table class="boldTablo" style="width: 100%">
        <tr>
            <th colspan="4">MÜŞTERİ/FİRMA BİLGİLERİ </th>
        </tr>
        <tr>
            <td style="width: 15%">Firma Adı:
            </td>
            <td style="width: 35%">
                <asp:Label ID="lblFirmaAdi" runat="server"></asp:Label>
            </td>
            <td style="width: 12%">Sipariş Adedi: </td>
            <td>
                <asp:Label ID="lblSiparisAdedi" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>Adı : </td>
            <td>
                <asp:Label ID="lblAd" runat="server"></asp:Label>
            </td>
            <td rowspan="5">Adresi : </td>
            <td rowspan="5">
                <asp:Label ID="lblAdres" runat="server"></asp:Label>
            </td>

        </tr>
        <tr>
            <td>Soyadı: </td>
            <td>
                <asp:Label ID="lblSoyad" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>

            <td>Ev Tel : </td>
            <td>
                <asp:Label ID="lblEvTel" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>iş Tel : </td>
            <td>
                <asp:Label ID="lblIsTel" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>Cep Tel : </td>
            <td>
                <asp:Label ID="lblCepTel" runat="server"></asp:Label>
            </td>
        </tr>
    </table>
    <table class="boldTablo" style="width: 100%">
        <tr>
            <th colspan="6">KAPI BİLGİLERİ </th>
        </tr>
        <tr>
            <td style="width: 13%">Dış Kapı Modeli :</td>
            <td style="width: 18%">
                <asp:Label ID="lblDisKapiModeli" runat="server"></asp:Label>
            </td>
            <td style="width: 12%">Kapı Cinsi:</td>
            <td style="width: 18%">
                <asp:Label ID="lblYanginKapiCins" runat="server"></asp:Label>
            </td>
            <td style="width: 12%">İç Kapı Modeli :</td>
            <td>
                <asp:Label ID="lblIcKapiModeli" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>Kasa Tipi :</td>
            <td>
                <asp:Label ID="lblYanginKasaTipi" runat="server"></asp:Label>
            </td>
            <td>Metal Rengi :</td>
            <td>
                <asp:Label ID="lblYanginMetalRengi" runat="server"></asp:Label>
            </td>
            <td>Eşik :</td>
            <td>
                <asp:Label ID="lblEsik" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td style="width: 10%">Kilit Sistemi :</td>
            <td>
                <asp:Label ID="lblKilitSistemi" runat="server"></asp:Label>
            </td>
            <td>Çekme Kolu :</td>
            <td>
                <asp:Label ID="lblCekmeKolu" runat="server"></asp:Label>
            </td>
            <td colspan="2"></td>
        </tr>
        <tr id="trYangin1" runat="server" visible="false">
            <td>Panik Bar :</td>
            <td>
                <asp:Label ID="lblYanginPanikBar" runat="server"></asp:Label>
            </td>
            <td>Kol ve Dış Müdahale Kolu :</td>
            <td>
                <asp:Label ID="lblYanginKol" runat="server"></asp:Label>
            </td>
            <td colspan="2"></td>
        </tr>
        <tr id="trYangin2" runat="server" visible="false">
            <td>Menteşe Tipi :</td>
            <td>
                <asp:Label ID="lblYanginMenteseTip" runat="server"></asp:Label>
            </td>
            <td>Hidrolik Kapatıcı :</td>
            <td>
                <asp:Label ID="lblYanginHidrolikKapatici" runat="server"></asp:Label>
            </td>
            <td colspan="2"></td>
        </tr>
        <tr id="trPorte1" runat="server" visible="false">
            <td>Cumba :</td>
            <td>
                <asp:Label ID="lblCumba" runat="server"></asp:Label>
            </td>
            <td>Dürbün :</td>
            <td>
                <asp:Label ID="Label1" runat="server"> </asp:Label>
            </td>
            <td colspan="2"></td>
        </tr>
        <tr id="trPorte2" runat="server" visible="false">
            <td>Barel :</td>
            <td>
                <asp:Label ID="Label2" runat="server"></asp:Label>
            </td>
            <td>Taktak :</td>
            <td>
                <asp:Label ID="Label3" runat="server"></asp:Label>
            </td>
            <td colspan="2"></td>
        </tr>
    </table>
    <table class="boldTablo" style="width: 100%;">
        <tr>
            <td rowspan="10" colspan="2" style="width: 20%">
                <telerik:RadBinaryImage ID="RadBinaryImage2" runat="server" ImageUrl="~/App_Themes/Theme/Raster/olcu3.png" 
                    Width="300" Height="250" />
            </td>
            <th colspan="4" style="text-align: center">ÖLÇÜM ve TESLIMAT</th>
        </tr>
        <tr>
            <td>Ölçümü Alan Kişi : </td>
            <td>
                <asp:Label ID="lblOlcumAlan" runat="server" RenderMode="Lightweight"></asp:Label>
            </td>
            <td>Teslim Tarihi:</td>
            <td>
                <asp:Label ID="lblTeslimTarihi" runat="server" RenderMode="Lightweight"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>Teslimat Şekli: </td>
            <td>
                <asp:Label ID="lblTeslimatSekli" runat="server" RenderMode="Lightweight"></asp:Label>
            </td>
            <td>Teslim Şekli: </td>
            <td>
                <asp:Label ID="lblTeslimSekli" runat="server" RenderMode="Lightweight"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>İç Kasa Genişliği:</td>
            <td>
                <asp:Label ID="lblIcKasaGenisligi" runat="server" RenderMode="Lightweight"></asp:Label>
            </td>
            <td>Dış Sol Pervaz:</td>
            <td>
                <asp:Label ID="lblDisSolPervaz" runat="server" RenderMode="Lightweight"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>İç Kasa Yüksekliği:</td>
            <td>
                <asp:Label ID="lblIcKasaYuksekligi" runat="server" RenderMode="Lightweight"></asp:Label>
            </td>
            <td>Dış Sağ Pervaz:</td>
            <td>
                <asp:Label ID="lblDisSagPervaz" runat="server" RenderMode="Lightweight"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>Duvar Kalınlığı:</td>
            <td>
                <asp:Label ID="lblDuvarKalinligi" runat="server" RenderMode="Lightweight"></asp:Label>
            </td>
            <td>Dış Üst Pervaz:</td>
            <td>
                <asp:Label ID="lblDisUstPervaz" runat="server" RenderMode="Lightweight"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>Açılım:</td>
            <td>
                <asp:Label ID="lblAcilim" runat="server" RenderMode="Lightweight"></asp:Label>
            </td>
            <td>İç Sol Pervaz:</td>
            <td>
                <asp:Label ID="lblIcSolPervaz" runat="server" RenderMode="Lightweight"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>Dış Kasa İç Pervaz Farkı:</td>
            <td>
                <asp:Label ID="lblDisKasaIcPervazFarki" runat="server" RenderMode="Lightweight"></asp:Label>
            </td>
            <td>İç Sağ Pervaz:</td>
            <td>
                <asp:Label ID="lblIcSagPervaz" runat="server" RenderMode="Lightweight"></asp:Label>
            </td>
        </tr>
        <tr>

            <td>İç Üst Pervaz:</td>
            <td>
                <asp:Label ID="lblIcUstPervaz" runat="server" RenderMode="Lightweight"></asp:Label>
            </td>
            <td colspan="2"></td>
        </tr>
        <tr>
            <td>Üretim Notları: </td>
            <td colspan="3">
                <asp:Label ID="lblOlcumBilgileri" runat="server" Widtd="100%" TextMode="MultiLine" RenderMode="Lightweight"></asp:Label>
            </td>
        </tr>
    </table>
    <table class="boldTablo" style="width: 100%">
        <tr>
            <th>NOT

            </th>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblNot" runat="server"></asp:Label>
                <br />
            </td>
        </tr>
    </table>
    <table class="boldTablo" runat="server" id="tbMusteriSozlesme" style="width: 100%">
        <tr>
            <th colspan="6">MÜŞTERİ SÖZLEŞMESİ </th>
        </tr>
        <tr>
            <td style="width: 15%">Adı Soyadı : </td>
            <td style="width: 35%">
                <asp:Label ID="lblMusteriAdSoyad" runat="server"></asp:Label>
            </td>
            <td>Vergi Numarası : </td>
            <td colspan="3">
                <asp:Label ID="lblVergiNumarasi" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>Adresi : </td>
            <td>
                <asp:Label ID="lblMusteriAdres" runat="server"></asp:Label>
            </td>
            <td></td>
            <td style="text-align: center">Peşin</td>
            <td style="text-align: center">Kalan</td>
            <td style="text-align: center">Ödeme Notu</td>
        </tr>
        <tr>
            <td>Cep Tel : </td>
            <td>
                <asp:Label ID="lblMusteriCepTel" runat="server"></asp:Label>
            </td>
            <td>Nakit:</td>
            <td>
                <asp:Label ID="lblNakitPesin" runat="server" CssClass="NumericFieldClass"></asp:Label>
            </td>
            <td>
                <asp:Label ID="lblNakitKalan" runat="server" CssClass="NumericFieldClass"></asp:Label>
            </td>
            <td>
                <asp:Label ID="lblNakitOdemeNotu" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>Fiyat : </td>
            <td>
                <asp:Label ID="lblFiyat" runat="server" CssClass="NumericFieldClass"></asp:Label>
            </td>
            <td>Kredi Kartı:</td>
            <td>
                <asp:Label ID="lblKKartiPesin" runat="server" CssClass="NumericFieldClass"></asp:Label>
            </td>
            <td>
                <asp:Label ID="lblKKartiKalan" runat="server" CssClass="NumericFieldClass"></asp:Label>
            </td>
            <td>
                <asp:Label ID="lblKKartiOdemeNotu" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>Vergi Dairesi : </td>
            <td>
                <asp:Label ID="lblVergiDairesi" runat="server"></asp:Label>
            </td>
            <td>Çek:</td>
            <td>
                <asp:Label ID="lblCekPesin" runat="server" CssClass="NumericFieldClass"></asp:Label>
            </td>
            <td>
                <asp:Label ID="lblCekKalan" runat="server" CssClass="NumericFieldClass"></asp:Label>
            </td>
            <td>
                <asp:Label ID="lblCekOdemeNotu" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td colspan="6">Yukarıda yazılı olan şartlarda sipariş verdim. İhtilaf halinde Ankara Mahkemeleri yetkilidir.
                    <b>Müşteri tarafından aksi yazılı olarak Ankara Çelik Kapı'ya bildirilmedikçe kapıların ölçüleri ve
                    açılış yönleri mevcut takılı olan kapıya göre imal edilecektir.</b>
                <br />
                <br />
            </td>
        </tr>
        <tr>
            <td colspan="6" style="text-align: center">
                <b>MÜŞTERİ
                       &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                       &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                       &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                       &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                       SİPARİŞ ALAN YETKİLİ</b>
            </td>
        </tr>
        <tr>
            <td colspan="6">
                <br />
                <br />
                <br />
                <br />
                <br />
            </td>
        </tr>
    </table>
</asp:Content>
