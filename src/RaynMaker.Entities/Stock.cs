﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Plainion.Validation;
using RaynMaker.Entities.Datums;

namespace RaynMaker.Entities
{
    public class Stock : EntityBase
    {
        private string myIsin;
        private string myWpkn;
        private string mySymbol;

        public Stock()
        {
            Prices = new List<Price>();
        }

        [Required]
        public string Isin
        {
            get { return myIsin; }
            set { SetProperty( ref myIsin, value ); }
        }

        public string Wpkn
        {
            get { return myWpkn; }
            set { SetProperty( ref myWpkn, value ); }
        }

        public string Symbol
        {
            get { return mySymbol; }
            set { SetProperty( ref mySymbol, value ); }
        }

        [Required]
        public virtual Company Company { get; set; }

        [ValidateObject]
        public virtual IList<Price> Prices { get; private set; }
    }
}
