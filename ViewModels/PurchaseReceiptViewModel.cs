using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VipcoSageX3.ViewModels
{
    public class PurchaseReceiptViewModel
    {
        #region PurchaseReceipt
        /// <summary>
        /// PRECEIPTD.PTHNUM
        /// </summary>
        public string RcNumber { get; set; }
        /// <summary>
        /// PRECEIPTD.PTDLIN
        /// </summary>
        public int? RcLine { get; set; }
        /// <summary>
        /// PRECEIPTD.RCPDAT
        /// </summary>
        public DateTime? RcDate { get; set; }
        public string RcDateString { get; set; }
        /// <summary>
        /// STOJOU.LOT_0 && STOJOU.SLO_0
        /// </summary>
        public string HeatNumber { get; set; }
        /// <summary>
        /// PRECEIPTD.PUU
        /// </summary>
        public string RcPurUom { get; set; }
        /// <summary>
        /// PRECEIPTD.STU
        /// </summary>
        public string RcStkUom { get; set; }
        /// <summary>
        /// PRECEIPTD.UOM
        /// </summary>
        public string RcUom { get; set; }
        /// <summary>
        /// PORDERQ.QTYPUU
        /// </summary>
        public double? RcQuantityPur { get; set; }
        /// <summary>
        /// PORDERQ.QTYSTU
        /// </summary>
        public double? RcQuantityStk { get; set; }
        /// <summary>
        /// PORDERQ.QTYUOM
        /// </summary>
        public double? RcQuantityUom { get; set; }
        /// <summary>
        /// PORDERQ.QTYWEU
        /// </summary>
        public double? RcQuantityWeight { get; set; }
        /// <summary>
        /// PORDERQ.INVQTYPUU
        /// </summary>
        public double? RcQuantityInvPur { get; set; }
        /// <summary>
        /// PORDERQ.INVQTYSTU
        /// </summary>
        public double? RcQuantityInvStk { get; set; }
        /// <summary>
        /// CPTANALIN.CCE0 WHERE VCRNUM = PTHNUM AND VCRLIN = PTDLIN 
        /// </summary>
        public string RcBranch { get; set; }
        public string RcBranchName { get; set; }
        /// <summary>
        /// CPTANALIN.CCE1 WHERE VCRNUM = PTHNUM AND VCRLIN = PTDLIN 
        /// </summary>
        public string RcWorkItem { get; set; }
        public string RcWorkItemName { get; set; }
        /// <summary>
        /// CPTANALIN.CCE3 WHERE VCRNUM = PTHNUM AND VCRLIN = PTDLIN 
        /// </summary>
        public string RcWorkGroup { get; set; }
        public string RcWorkGroupName { get; set; }
        /// <summary>
        /// CPTANALIN.CCE2 WHERE VCRNUM = PTHNUM AND VCRLIN = PTDLIN 
        /// </summary>
        public string RcProject { get; set; }
        public string RcProjectName { get; set; }
        #endregion
    }
}
