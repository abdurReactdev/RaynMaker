﻿using System.ComponentModel.DataAnnotations;

namespace RaynMaker.Blade.DataSheetSpec
{
    public class DatumWithCurrency : Datum
    {
        [Required]
        public Currency Curreny { get; set; }
    }
}
