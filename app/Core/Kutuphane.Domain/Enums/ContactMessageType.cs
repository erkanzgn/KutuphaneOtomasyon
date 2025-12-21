using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kutuphane.Domain.Enums
{
    public enum ContactMessageType
    {
        [Display(Name = "Genel Soru")]
        Genel = 0,

        [Display(Name = "Öneri")]
        Oneri = 1,

        [Display(Name = "Şikayet")]
        Sikayet = 2,

        [Display(Name = "Teknik Destek")]
        Destek = 3,

        [Display(Name = "Kitap Talebi")]
        KitapTalebi = 4,

        [Display(Name = "Giden Mesaj")]
        Sent




    }
}
