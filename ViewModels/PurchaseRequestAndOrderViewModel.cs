﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VipcoSageX3.ViewModels
{
    public class PurchaseRequestAndOrderViewModel
    {
        #region PurchaseRequest
        //Purchase Request
        public DateTime ToDate { get; set; }
        public bool DeadLine { get; set; }
        /// <summary>
        /// PREQUISD.PSHNUM
        /// </summary>
        public string PrNumber { get; set; }
        /// <summary>
        /// PREQUISD.PSDLIN
        /// </summary>
        public int? PrLine { get; set; }
        /// <summary>
        /// PREQUISD.ITMREF
        /// </summary>
        public string ItemCode { get; set; }
        /// <summary>
        /// PREQUISD.ITMDES
        /// </summary>
        public string ItemName { get; set; }
        /// <summary>
        /// PREQUISD.PUU
        /// </summary>
        public string PurUom { get; set; }
        /// <summary>
        /// PREQUISD.STU
        /// </summary>
        public string StkUom { get; set; }
        /// <summary>
        /// CPTANALIN.CCE0 WHERE VCRNUM = PSHNUM AND VCRLIN = PSDLIN
        /// </summary>
        public string Branch { get; set; }
        public string BranchName { get; set; }
        /// <summary>
        /// CPTANALIN.CCE1 WHERE VCRNUM = PSHNUM AND VCRLIN = PSDLIN
        /// </summary>
        public string WorkItem { get; set; }
        public string WorkItemName { get; set; }
        /// <summary>
        /// CPTANALIN.CCE3 WHERE VCRNUM = PSHNUM AND VCRLIN = PSDLIN
        /// </summary>
        public string WorkGroup { get; set; }
        public string WorkGroupName { get; set; }
        /// <summary>
        /// CPTANALIN.CCE2 WHERE VCRNUM = PSHNUM AND VCRLIN = PSDLIN
        /// </summary>
        public string Project { get; set; }
        public string ProjectName { get; set; }
        /// <summary>
        /// PREQUISD.QTYPUU
        /// </summary>
        public double? QuantityPur { get; set; }
        /// <summary>
        /// PREQUISD.QTYSTU
        /// </summary>
        public double? QuantityStk { get; set; }
        /// <summary>
        /// PREQUISD.EXTRCPDAT
        /// </summary>
        public DateTime? PRDate { get; set; }
        public string PRDateString { get; set; }
        /// <summary>
        /// PREQUISO.POHNUM
        /// </summary>
        public string LinkPoNumber { get; set; }
        /// <summary>
        /// PREQUISO.POPLIN
        /// </summary>
        public int? LinkPoLine { get; set; }
        /// <summary>
        /// PREQUISO.POQSEQ
        /// </summary>
        public int? LinkPoSEQ { get; set; }
        /// <summary>
        /// PREQUIS.CREUSR
        /// </summary>
        public string CreateBy { get; set; }
        /// <summary>
        /// PREQUIS.CLEFLG_0
        /// </summary>
        public string PrCloseStatus { get; set; }
        #endregion

        #region PurchaseOrder
        // Purchase Order
        /// <summary>
        /// PORDERQ.POHNUM
        /// </summary>
        public string PoNumber { get; set; }
        /// <summary>
        /// PORDERQ.POPLIN
        /// </summary>
        public int? PoLine { get; set; }
        /// <summary>
        /// PORDERQ.POQSEQ
        /// </summary>
        public int? PoSequence { get; set; }
        /// <summary>
        /// PORDER.ZPO210
        /// </summary>
        public string PoStatus { get; set; }
        /// <summary>
        /// PORDER.CLEFLG
        /// </summary>
        public string CloseStatus { get; set; }
        /// <summary>
        /// PORDERQ.ORDDAT
        /// </summary>
        public DateTime? PoDate { get; set; }
        public string PoDateString { get; set; }
        /// <summary>
        /// PORDERQ.EXTRCPDAT 
        /// </summary>
        public DateTime? DueDate { get; set; }
        public string DueDateString { get; set; }
        /// <summary>
        /// PORDERQ.PUU
        /// </summary>
        public string PoPurUom { get; set; }
        /// <summary>
        /// PORDERQ.STU
        /// </summary>
        public string PoStkUom { get; set; }
        /// <summary>
        /// PORDERQ.QTYPUU
        /// </summary>
        public double? PoQuantityPur { get; set; }
        /// <summary>
        /// PORDERQ.QTYSTU
        /// </summary>
        public double? PoQuantityStk { get; set; }
        /// <summary>
        /// PORDERQ.QTYWEU
        /// </summary>
        public double? PoQuantityWeight { get; set; }
        /// <summary>
        /// CPTANALIN.CCE0 WHERE VCRNUM = POHNUM AND VCRLIN = POPLIN AND VCRSEQ = POQSEQ
        /// </summary>
        public string PoBranch { get; set; }
        public string PoBranchName { get; set; }
        /// <summary>
        /// CPTANALIN.CCE1 WHERE VCRNUM = POHNUM AND VCRLIN = POPLIN AND VCRSEQ = POQSEQ
        /// </summary>
        public string PoWorkItem { get; set; }
        public string PoWorkItemName { get; set; }
        /// <summary>
        /// CPTANALIN.CCE3 WHERE VCRNUM = PSHNUM AND VCRLIN = PSDLIN AND VCRSEQ = POQSEQ
        /// </summary>
        public string PoWorkGroup { get; set; }
        public string PoWorkGroupName { get; set; }
        /// <summary>
        /// CPTANALIN.CCE2 WHERE VCRNUM = PSHNUM AND VCRLIN = PSDLIN AND VCRSEQ = POQSEQ
        /// </summary>
        public string PoProject { get; set; }
        public string PoProjectName { get; set; }

        #endregion

        #region PurchaseReceipts
        public ICollection<PurchaseReceiptViewModel> PurchaseReceipts { get; set; } = new List<PurchaseReceiptViewModel>();

        #endregion

    }
}
