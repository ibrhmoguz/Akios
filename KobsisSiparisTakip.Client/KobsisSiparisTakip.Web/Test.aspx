﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Test.aspx.cs" Inherits="KobsisSiparisTakip.Web.Test" Trace="false" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="App_Themes/Theme/Template/Template.css" type="text/css" rel="stylesheet" />
    <link href="App_Themes/Theme/StyleSheet.css" type="text/css" rel="stylesheet" />

</head>
<body>
    <form id="form1" runat="server">
        <telerik:RadScriptManager runat="server" ID="RadScriptManager1" />
        <br />
        <table class="AnaTablo" style="width: 100%">
            <tr>
                <td rowspan="6" style="text-align: center">
                    <asp:Image ID="imgFirmaLogo" runat="server" src="" />
                </td>
                <td colspan="2" rowspan="3" style="width: 45%; font-size: x-large; text-align: center; font-weight: 700">
                    <b>
                        <asp:Label ID="lblKapiSeri1" runat="server"></asp:Label></b>
                </td>
                <td style="width: 30%; text-align: left">
                    <b>
                        <asp:Label ID="lblFirmaAdi" runat="server"></asp:Label>
                    </b>
                </td>
            </tr>
            <tr>

                <td style="font-size: xx-small; text-align: left">
                    <b>Adres: </b>
                    <asp:Label ID="lblFirmaAdres" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td style="font-size: xx-small; text-align: left">
                    <b>Telefon: </b>
                    <asp:Label ID="lblFirmaTelefon" runat="server"></asp:Label>
                </td>

            </tr>
            <tr>
                <td colspan="2" rowspan="3" style="font-size: x-large; text-align: center">
                    <b>SİPARİŞ FORMU</b>
                </td>
                <td style="font-size: xx-small; text-align: left">
                    <b>Faks : </b>
                    <asp:Label ID="lblFirmaFaks" runat="server"></asp:Label>
                </td>

            </tr>
            <tr>
                <td style="font-size: xx-small; text-align: left">
                    <b>Web : </b>
                    <asp:Label ID="lblFirmaWebAdres" runat="server"></asp:Label>
                </td>

            </tr>
            <tr>
                <td style="font-size: xx-small; text-align: left">
                    <b>e-posta : </b>
                    <asp:Label ID="lblFirmaMail" runat="server"></asp:Label>
                </td>
            </tr>
        </table>
        <br />
        <table class="AnaTablo" style="width: 100%">
            <tr>
                <th colspan="4">MÜŞTERİ/FİRMA BİLGİLERİ </th>
            </tr>
            <tr>
                <th style="width: 15%">Firma Adı:
                </th>
                <td style="width: 35%">
                    <telerik:RadTextBox ID="txtMusteriFirmaAdi" runat="server" RenderMode="Lightweight"></telerik:RadTextBox>
                </td>
                <th style="width: 10%">Sipariş Adedi: </th>
                <td>
                    <telerik:RadTextBox ID="txtSiparisAdedi" runat="server" Text="1" RenderMode="Lightweight"></telerik:RadTextBox>
                </td>
            </tr>
            <tr>
                <th>Adı : </th>
                <td>
                    <telerik:RadTextBox ID="txtMusteriAd" runat="server" RenderMode="Lightweight"></telerik:RadTextBox>
                </td>
                <th rowspan="5">Adresi : </th>
                <td rowspan="5">
                    <table style="width: 100%">
                        <tr>
                            <td colspan="2">
                                <telerik:RadTextBox ID="txtMusteriAdres" runat="server" TextMode="MultiLine" Height="50px" Width="250px" RenderMode="Lightweight"></telerik:RadTextBox>
                            </td>
                        </tr>
                        <tr>
                            <th>İl :</th>
                            <td>
                                <telerik:RadComboBox ID="ddlMusteriIl" runat="server" AutoPostBack="true" EmptyMessage="İl Seçiniz" Skin="Telerik" OnSelectedIndexChanged="ddlMusteriIl_SelectedIndexChanged">
                                </telerik:RadComboBox>
                            </td>
                        </tr>
                        <tr>
                            <th>İlçe :</th>
                            <td>
                                <telerik:RadComboBox ID="ddlMusteriIlce" runat="server" AutoPostBack="True" EmptyMessage="İlçe Seçiniz" RenderMode="Lightweight" OnSelectedIndexChanged="ddlMusteriIlce_SelectedIndexChanged">
                                </telerik:RadComboBox>
                            </td>
                        </tr>
                        <tr>
                            <th>Semt :</th>
                            <td>
                                <telerik:RadComboBox ID="ddlMusteriSemt" runat="server" AutoPostBack="false" EmptyMessage="Semt Seçiniz" RenderMode="Lightweight">
                                </telerik:RadComboBox>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <th style="width: 10%">Soyadı </th>
                <td>
                    <telerik:RadTextBox ID="txtMusteriSoyad" runat="server" RenderMode="Lightweight"></telerik:RadTextBox>
                </td>
            </tr>
            <tr>
                <th>Ev Tel : </th>
                <td>
                    <telerik:RadMaskedTextBox ID="txtMusteriEvTel" runat="server" Mask="(###) ### ## ##" RenderMode="Lightweight"></telerik:RadMaskedTextBox>
                </td>
            </tr>
            <tr>
                <th>iş Tel : </th>
                <td>
                    <telerik:RadMaskedTextBox ID="txtMusteriIsTel" runat="server" Mask="(###) ### ## ##" RenderMode="Lightweight"></telerik:RadMaskedTextBox>
                </td>
            </tr>
            <tr>
                <th>Cep Tel : </th>
                <td>
                    <telerik:RadMaskedTextBox ID="txtMusteriCepTel" runat="server" Mask="(###) ### ## ##" RenderMode="Lightweight"></telerik:RadMaskedTextBox>
                </td>
            </tr>
        </table>
        <asp:Panel ID="divSiparisFormKontrolleri" runat="server" Width="100%"></asp:Panel>
        <table style="width: 100%">
            <tr>
                <td style="text-align: center">
                    <br />
                    <telerik:RadButton ID="btnKaydet" runat="server" Text="Kaydet" OnClick="btnKaydet_Click">
                        <Icon PrimaryIconCssClass="rbSave" PrimaryIconLeft="4" PrimaryIconTop="3" />
                    </telerik:RadButton>
                </td>
            </tr>
        </table>
        <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server" DefaultLoadingPanelID="RadAjaxLoadingPanel1">
            <AjaxSettings>
                <telerik:AjaxSetting AjaxControlID="ddlMusteriIl">
                    <UpdatedControls>
                        <telerik:AjaxUpdatedControl ControlID="ddlMusteriIlce" />
                        <telerik:AjaxUpdatedControl ControlID="ddlMusteriSemt" />
                    </UpdatedControls>
                </telerik:AjaxSetting>
                <telerik:AjaxSetting AjaxControlID="ddlMusteriIlce">
                    <UpdatedControls>
                        <telerik:AjaxUpdatedControl ControlID="ddlMusteriSemt" />
                    </UpdatedControls>
                </telerik:AjaxSetting>
            </AjaxSettings>
        </telerik:RadAjaxManager>
        <telerik:RadAjaxLoadingPanel ID="RadAjaxLoadingPanel1" runat="server" />
    </form>
</body>
</html>
