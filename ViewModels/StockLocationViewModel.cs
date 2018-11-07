using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VipcoSageX3.ViewModels
{
    public class StockLocationViewModel
    {
        /// <summary>
        /// STOCK.LOC_0
        /// </summary>
        public string LocationCode { get; set; }
        /// <summary>
        /// STOCK.QTYPCU_0
        /// </summary>
        public double Quantity { get; set; }
        public string QuantityString => string.Format("{0:#,##0}", this.Quantity);
        /// <summary>
        /// STOCK.PJT_0
        /// </summary>
        public string Project { get; set; }
    }
}
