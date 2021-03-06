﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using Akios.DataType;

namespace Akios.Util
{
    public static class SessionManager
    {
        public static void Remove(string id)
        {
            if (HttpContext.Current.Session[id] != null)
            {
                HttpContext.Current.Session.Remove(id);
            }
        }

        public static void Clear()
        {
            HttpContext.Current.Session.Clear();
        }

        public static DataTable ReferansData
        {
            get
            {
                if (HttpContext.Current.Session["ReferansData"] != null)
                    return (DataTable)HttpContext.Current.Session["ReferansData"];

                return null;
            }
            set
            {
                HttpContext.Current.Session["ReferansData"] = value;
            }
        }


        public static List<Layout> SiparisFormLayout
        {
            get
            {
                if (HttpContext.Current.Session["SiparisFormLayout"] != null)
                    return (List<Layout>)HttpContext.Current.Session["SiparisFormLayout"];

                return null;
            }
            set
            {
                HttpContext.Current.Session["SiparisFormLayout"] = value;
            }
        }

        public static List<Layout> SiparisSorgulaFormLayout
        {
            get
            {
                if (HttpContext.Current.Session["SiparisSorgulaFormLayout"] != null)
                    return (List<Layout>)HttpContext.Current.Session["SiparisSorgulaFormLayout"];

                return null;
            }
            set
            {
                HttpContext.Current.Session["SiparisSorgulaFormLayout"] = value;
            }
        }

        public static DataTable SiparisBilgi
        {
            get
            {
                if (HttpContext.Current.Session["SiparisBilgi"] != null)
                    return (DataTable)HttpContext.Current.Session["SiparisBilgi"];

                return null;
            }
            set
            {
                HttpContext.Current.Session["SiparisBilgi"] = value;
            }
        }

        public static Musteri MusteriBilgi
        {
            get
            {
                if (HttpContext.Current.Session["MusteriBilgi"] != null)
                    return (Musteri)HttpContext.Current.Session["MusteriBilgi"];

                return null;
            }
            set
            {
                HttpContext.Current.Session["MusteriBilgi"] = value;
            }
        }

        public static Kullanici KullaniciBilgi
        {
            get
            {
                if (HttpContext.Current.Session["KullaniciBilgi"] != null)
                    return (Kullanici)HttpContext.Current.Session["KullaniciBilgi"];

                return null;
            }
            set
            {
                HttpContext.Current.Session["KullaniciBilgi"] = value;
            }
        }

        public static string LoginAttemptUser
        {
            get
            {
                if (HttpContext.Current.Session["LoginAttemptUser"] != null)
                    return HttpContext.Current.Session["LoginAttemptUser"].ToString();

                return null;
            }
            set
            {
                HttpContext.Current.Session["LoginAttemptUser"] = value;
            }
        }

        public static int? LoginAttemptCount
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(HttpContext.Current.Session["LoginAttemptCount"].ToString()))
                    return Convert.ToInt32(HttpContext.Current.Session["LoginAttemptCount"].ToString());

                return null;
            }
            set
            {
                HttpContext.Current.Session["LoginAttemptCount"] = value;
            }
        }

        public static string CaptchaImageText
        {
            get
            {
                if (HttpContext.Current.Session["CaptchaImageText"] != null)
                    return HttpContext.Current.Session["CaptchaImageText"].ToString();

                return null;
            }
            set
            {
                HttpContext.Current.Session["CaptchaImageText"] = value;
            }
        }

        public static List<SiparisSeri> SiparisSeri
        {
            get
            {
                if (HttpContext.Current.Session["SiparisSeri"] != null)
                    return (List<SiparisSeri>)HttpContext.Current.Session["SiparisSeri"];

                return null;
            }
            set
            {
                HttpContext.Current.Session["SiparisSeri"] = value;
            }
        }

        public static string TeslimatKotaKontrolu
        {
            get
            {
                if (HttpContext.Current.Session["TeslimatKotaKontrolu"] != null)
                    return HttpContext.Current.Session["TeslimatKotaKontrolu"].ToString();

                return string.Empty;
            }
            set
            {
                HttpContext.Current.Session["TeslimatKotaKontrolu"] = value;
            }
        }

        public static int TeslimatKotaVarsayilan
        {
            get
            {
                if (HttpContext.Current.Session["TeslimatKotaVarsayilan"] != null && !string.IsNullOrWhiteSpace(HttpContext.Current.Session["TeslimatKotaVarsayilan"].ToString()))
                    return Convert.ToInt32(HttpContext.Current.Session["TeslimatKotaVarsayilan"].ToString());

                return 0;
            }
            set
            {
                HttpContext.Current.Session["TeslimatKotaVarsayilan"] = value;
            }
        }

        public static DataTable PersonelListesi
        {
            get
            {
                if (HttpContext.Current.Session["PersonelListesi"] != null)
                    return (DataTable)HttpContext.Current.Session["PersonelListesi"];

                return null;
            }
            set
            {
                HttpContext.Current.Session["PersonelListesi"] = value;
            }
        }

        public static DataTable SiparisSorguListesi
        {
            get
            {
                if (HttpContext.Current.Session["SiparisSorguListesi"] != null)
                    return (DataTable)HttpContext.Current.Session["SiparisSorguListesi"];

                return null;
            }
            set
            {
                HttpContext.Current.Session["SiparisSorguListesi"] = value;
            }
        }

        public static DataTable TeslimatListesi
        {
            get
            {
                if (HttpContext.Current.Session["Takvim_TeslimatListesi"] != null)
                    return HttpContext.Current.Session["Takvim_TeslimatListesi"] as DataTable;

                return null;
            }
            set
            {
                HttpContext.Current.Session["Takvim_TeslimatListesi"] = value;
            }
        }

        public static DataTable MusteriReferansDegerleri
        {
            get
            {
                if (HttpContext.Current.Session["MusteriReferansDegerleri"] != null)
                    return HttpContext.Current.Session["MusteriReferansDegerleri"] as DataTable;

                return null;
            }
            set
            {
                HttpContext.Current.Session["MusteriReferansDegerleri"] = value;
            }
        }

        public static DataTable ReferansDetay
        {
            get
            {
                if (HttpContext.Current.Session["ReferansDetay"] != null)
                    return (DataTable)HttpContext.Current.Session["ReferansDetay"];

                return null;
            }
            set
            {
                HttpContext.Current.Session["ReferansDetay"] = value;
            }
        }

        public static DataTable GunlukIsTakipListesi
        {
            get
            {
                if (HttpContext.Current.Session["GunlukIsTakipListesi"] != null)
                    return (DataTable)HttpContext.Current.Session["GunlukIsTakipListesi"];

                return null;
            }
            set
            {
                HttpContext.Current.Session["GunlukIsTakipListesi"] = value;
            }
        }

        public static DataTable SatisAdetListesi
        {
            get
            {
                if (HttpContext.Current.Session["SatisAdetListesi"] != null)
                    return (DataTable)HttpContext.Current.Session["SatisAdetListesi"];

                return null;
            }
            set
            {
                HttpContext.Current.Session["SatisAdetListesi"] = value;
            }
        }

        public static DataTable SatisTutarListesi
        {
            get
            {
                if (HttpContext.Current.Session["SatisTutarListesi"] != null)
                    return (DataTable)HttpContext.Current.Session["SatisTutarListesi"];

                return null;
            }
            set
            {
                HttpContext.Current.Session["SatisTutarListesi"] = value;
            }
        }

        public static DataSet IlIlceyeGoreSatilanAdet
        {
            get
            {
                if (HttpContext.Current.Session["IlIlceyeGoreSatilanAdet"] != null)
                    return (DataSet)HttpContext.Current.Session["IlIlceyeGoreSatilanAdet"];

                return null;
            }
            set
            {
                HttpContext.Current.Session["IlIlceyeGoreSatilanAdet"] = value;
            }
        }

        public static List<MusteriRapor> MusteriRaporlar
        {
            get
            {
                if (HttpContext.Current.Session["MusteriRaporlar"] != null)
                    return (List<MusteriRapor>)HttpContext.Current.Session["MusteriRaporlar"];

                return null;
            }
            set
            {
                HttpContext.Current.Session["MusteriRaporlar"] = value;
            }
        }

        public static DataTable MusteriListesi
        {
            get
            {
                if (HttpContext.Current.Session["MusteriListesi"] != null)
                    return (DataTable)HttpContext.Current.Session["MusteriListesi"];

                return null;
            }
            set
            {
                HttpContext.Current.Session["MusteriListesi"] = value;
            }
        }

        public static Imaj MusteriLogo
        {
            get
            {
                if (HttpContext.Current.Session["MusteriLogo"] != null)
                    return (Imaj)HttpContext.Current.Session["MusteriLogo"];

                return null;
            }
            set
            {
                HttpContext.Current.Session["MusteriLogo"] = value;
            }
        }
    }
}