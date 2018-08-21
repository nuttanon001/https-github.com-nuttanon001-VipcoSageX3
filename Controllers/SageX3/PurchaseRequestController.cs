using System;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

using VipcoSageX3.Services;
using VipcoSageX3.ViewModels;
using VipcoSageX3.Models.SageX3;

using AutoMapper;
using VipcoSageX3.Helper;
using RtfPipe;

namespace VipcoSageX3.Controllers.SageX3
{
    // : api/PurchaseRequest
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseRequestController : GenericSageX3Controller<Prequisd>
    {
        private readonly IRepositorySageX3<Cacce> repositoryDim;
        private readonly IRepositorySageX3<Prequiso> repositoryPRLink;
        private readonly IRepositorySageX3<Porderq> repositoryPOLine;
        private readonly IRepositorySageX3<Porder> repositoryPoHeader;
        private readonly IRepositorySageX3<Preceiptd> repositoryRCLine;
        private readonly IRepositorySageX3<Cptanalin> repositoryDimLink;
        //Context
        private readonly SageX3Context sageContext;
        // GET: api/PurchaseRequest
        public PurchaseRequestController(
            IRepositorySageX3<Prequisd> repo,
            IRepositorySageX3<Prequiso> repoPrLink,
            IRepositorySageX3<Porderq> repoPoLine,
            IRepositorySageX3<Preceiptd> repoRcLine,
            IRepositorySageX3<Cptanalin> repoDimLink,
             IRepositorySageX3<Porder> repoPoHeader,
            IRepositorySageX3<Cacce> repoDim,
            SageX3Context x3Context,
            IMapper mapper) : base(repo, mapper)
        {
            // Repository SageX3
            this.repositoryDim = repoDim;
            this.repositoryDimLink = repoDimLink;
            this.repositoryPoHeader = repoPoHeader;
            this.repositoryPOLine = repoPoLine;
            this.repositoryPRLink = repoPrLink;
            this.repositoryRCLine = repoRcLine;
            // Context
            this.sageContext = x3Context;
        }

        // POST: api/PurchaseRequest/GetScroll
        [HttpPost("GetScroll")]
        public async Task<IActionResult> GetScroll([FromBody] ScrollViewModel Scroll)
        {
            if (Scroll == null)
                return BadRequest();

            var QueryData = (from prh in this.sageContext.Prequis
                             join prd in this.sageContext.Prequisd on prh.Pshnum0 equals prd.Pshnum0 into new_pr
                             from all1 in new_pr.DefaultIfEmpty()
                             join itm in this.sageContext.Itmmaster on all1.Itmref0 equals itm.Itmref0 into new_pr_itm
                             from all2 in new_pr_itm.DefaultIfEmpty()
                             join dim in this.sageContext.Cptanalin on new { key1 = all1.Pshnum0, key2 = all1.Psdlin0 } equals new { key1 = dim.Vcrnum0, key2 = dim.Vcrlin0 } into new_pr_dim
                             from all3 in new_pr_dim.DefaultIfEmpty()
                             join pro in this.sageContext.Prequiso on new { all1.Pshnum0, all1.Psdlin0 } equals new { pro.Pshnum0, pro.Psdlin0 } into new_pr_lik
                             from all4 in new_pr_lik.DefaultIfEmpty()
                             join pod in this.sageContext.Porderq on new { all4.Pohnum0, all4.Poplin0 } equals new { pod.Pohnum0, pod.Poplin0 } into new_pr_po
                             from all5 in new_pr_po.DefaultIfEmpty()
                             join dim in this.sageContext.Cptanalin on new { key1 = all5.Pohnum0, key2 = all5.Poplin0 } equals new { key1 = dim.Vcrnum0, key2 = dim.Vcrlin0 } into new_po_dim
                             from all6 in new_po_dim.DefaultIfEmpty()
                             join tex in this.sageContext.Texclob on new { Code0 = all2.Purtex0 } equals new { tex.Code0 } into new_pur_tex
                             from all7 in new_pur_tex.DefaultIfEmpty()
                             where all2.Tclcod0.StartsWith('1') //&& (all4.Pohnum0 == "PO1808-0006" || all4.Pohnum0 == "PO1808-0008" || all4.Pohnum0 == "PO1808-0072")
                             select new
                             {
                                 item = all2,
                                 item_tex = all7,
                                 pod = all5,
                                 prd = all1,
                                 prh,
                                 pro = all4,
                                 dim = all3,
                                 dimPo = all6
                             }).AsQueryable();
            #region Query
            
            #endregion

            // Filter
            var filters = string.IsNullOrEmpty(Scroll.Filter) ? new string[] { "" }
                                : Scroll.Filter.Split(null);
            foreach (string temp in filters)
            {
                string keyword = temp.ToLower();
                QueryData = QueryData.Where(x => x.prd.Pjt0.ToLower().Contains(keyword) ||
                                                 x.prd.Itmref0.ToLower().Contains(keyword) ||
                                                 x.prd.Itmdes0.ToLower().Contains(keyword) ||
                                                 x.prd.Pshnum0.ToLower().Contains(keyword) ||
                                                 x.pod.Pohnum0.ToLower().Contains(keyword) ||
                                                 x.pod.Itmref0.ToLower().Contains(keyword));
            }

            if (!string.IsNullOrEmpty(Scroll.WhereBranch))
                QueryData = QueryData.Where(x => x.dim.Cce0 == Scroll.WhereBranch);

            if (!string.IsNullOrEmpty(Scroll.WhereWorkGroup))
                QueryData = QueryData.Where(x => x.dim.Cce3 == Scroll.WhereWorkGroup);

            if (!string.IsNullOrEmpty(Scroll.WhereWorkItem))
                QueryData = QueryData.Where(x => x.dim.Cce1 == Scroll.WhereWorkItem);

            if (!string.IsNullOrEmpty(Scroll.WhereProject))
                QueryData = QueryData.Where(x => x.dim.Cce2 == Scroll.WhereProject);

            if (Scroll.SDate.HasValue && Scroll.EDate.HasValue)
            {
                QueryData = QueryData.Where(x =>
                    x.prh.Prqdat0.Date >= Scroll.SDate.Value.Date &&
                    x.prh.Prqdat0.Date <= Scroll.EDate.Value.Date);
            }

            //MapData.Branch = PrDim.Cce0;
            //MapData.WorkItem = PrDim.Cce1;
            //MapData.Project = PrDim.Cce2;
            //MapData.WorkGroup = PrDim.Cce3;
            switch (Scroll.SortField)
            {
                case "PrNumber":
                    if (Scroll.SortOrder == -1)
                        QueryData = QueryData.OrderByDescending(x => x.prh.Pshnum0);
                    else
                        QueryData = QueryData.OrderBy(x => x.prh.Pshnum0);
                    break;
                case "Project":
                    if (Scroll.SortOrder == -1)
                        QueryData = QueryData.OrderByDescending(x => x.prh.Pjth0);
                    else
                        QueryData = QueryData.OrderBy(x => x.prh.Pjth0);
                    break;
                case "PRDateString":
                    if (Scroll.SortOrder == -1)
                        QueryData = QueryData.OrderByDescending(x => x.prh.Prqdat0);
                    else
                        QueryData = QueryData.OrderBy(x => x.prh.Prqdat0);
                    break;
                case "ItemName":
                    if (Scroll.SortOrder == -1)
                        QueryData = QueryData.OrderByDescending(x => x.prd.Itmdes0);
                    else
                        QueryData = QueryData.OrderBy(x => x.prd.Itmdes0);
                    break;
                case "Branch":
                    if (Scroll.SortOrder == -1)
                        QueryData = QueryData.OrderByDescending(x => x.dim.Cce0);
                    else
                        QueryData = QueryData.OrderBy(x => x.dim.Cce0);
                    break;
                case "WorkItemName":
                    if (Scroll.SortOrder == -1)
                        QueryData = QueryData.OrderByDescending(x => x.dim.Cce1);
                    else
                        QueryData = QueryData.OrderBy(x => x.dim.Cce1);
                    break;
                case "WorkGroupName":
                    if (Scroll.SortOrder == -1)
                        QueryData = QueryData.OrderByDescending(x => x.dim.Cce3);
                    else
                        QueryData = QueryData.OrderBy(x => x.dim.Cce3);
                    break;
                case "PoNumber":
                    if (Scroll.SortOrder == -1)
                        QueryData = QueryData.OrderByDescending(x => x.pod.Pohnum0);
                    else
                        QueryData = QueryData.OrderBy(x => x.pod.Pohnum0);
                    break;
                case "PoDateString":
                    if (Scroll.SortOrder == -1)
                        QueryData = QueryData.OrderByDescending(x => x.pod.Orddat0);
                    else
                        QueryData = QueryData.OrderBy(x => x.pod.Orddat0);
                    break;
                case "DueDateString":
                    if (Scroll.SortOrder == -1)
                        QueryData = QueryData.OrderByDescending(x => x.pod.Extrcpdat0);
                    else
                        QueryData = QueryData.OrderBy(x => x.pod.Extrcpdat0);
                    break;
                default:
                    QueryData = QueryData.OrderByDescending(x => x.prh.Prqdat0);
                    break;
            }
            Scroll.TotalRow = await QueryData.CountAsync();
            var Message = "";
            try
            {
                var Datasource = await QueryData.Skip(Scroll.Skip ?? 0).Take(Scroll.Take ?? 15).AsNoTracking().ToListAsync();
                var MapDatas = new List<PurchaseRequestAndOrderViewModel>();
                var ListCCE = new List<string>();

                foreach (var item in Datasource)
                {
                    var MapData = this.mapper.Map<Prequis, PurchaseRequestAndOrderViewModel>(item.prh);
                    MapData.ToDate = DateTime.Today;

                    this.mapper.Map<Prequisd, PurchaseRequestAndOrderViewModel>(item.prd, MapData);
                    //ItemName
                    if (item.item_tex?.Texte0 != null)
                    {
                        if (item.item_tex.Texte0.StartsWith("{\\rtf1"))
                            MapData.ItemName = Rtf.ToHtml(item.item_tex?.Texte0);
                        else
                            MapData.ItemName = item?.item_tex?.Texte0;
                    }
                    else
                        MapData.ItemName = item?.item_tex?.Texte0 ?? MapData.ItemName;

                    // Get Dimension for PurchaseRequest1
                    if (item.dim != null)
                    {
                        MapData.Branch = item.dim.Cce0;
                        MapData.WorkItem = item.dim.Cce1;
                        MapData.Project = item.dim.Cce2;
                        MapData.WorkGroup = item.dim.Cce3;
                        // Add CCE
                        ListCCE.Add(item.dim.Cce0);
                        ListCCE.Add(item.dim.Cce1);
                        ListCCE.Add(item.dim.Cce2);
                        ListCCE.Add(item.dim.Cce3);
                    }

                    // Get Purchase Request Link
                    if (item.pro != null)
                    {
                        this.mapper.Map<Prequiso, PurchaseRequestAndOrderViewModel>(item.pro, MapData);
                        // Get Purchase Order
                        if (item.pod != null)
                        {
                            // Get Status PurchaseOrder
                            var PoHeader = await this.repositoryPoHeader.GetFirstOrDefaultAsync(x => new Porder()
                            {
                                Pohnum0 = x.Pohnum0,
                                Zpo210 = x.Zpo210,
                                Cleflg0 = x.Cleflg0,
                                Rowid = x.Rowid
                            },x => x.Pohnum0 == item.pod.Pohnum0);

                            if (PoHeader != null)
                                this.mapper.Map<Porder, PurchaseRequestAndOrderViewModel>(PoHeader, MapData);

                            this.mapper.Map<Porderq, PurchaseRequestAndOrderViewModel>(item.pod, MapData);
                            if (item.dimPo != null)
                            {
                                MapData.PoBranch = item.dimPo.Cce0;
                                MapData.PoWorkItem = item.dimPo.Cce1;
                                MapData.PoProject = item.dimPo.Cce2;
                                MapData.PoWorkGroup = item.dimPo.Cce3;
                                // Add CCE
                                ListCCE.Add(item.dimPo.Cce0);
                                ListCCE.Add(item.dimPo.Cce1);
                                ListCCE.Add(item.dimPo.Cce2);
                                ListCCE.Add(item.dimPo.Cce3);
                            }

                            //var ListRcs = await this.repositoryRCLine.GetToListAsync(
                            //    x => x,
                            //    x => x.Pohnum0 == item.pod.Pohnum0 && x.Poplin0 == item.pod.Poplin0 && x.Poqseq0 == item.pod.Poqseq0);

                            var ReciptionLine = await (from prc in this.sageContext.Preceiptd
                                                        join sto in this.sageContext.Stojou on new { key1 = prc.Pthnum0, key2 = prc.Ptdlin0 } equals new { key1 = sto.Vcrnum0, key2 = sto.Vcrlin0 } into new_sto
                                                        from all1 in new_sto.DefaultIfEmpty()
                                                        where prc.Pohnum0 == item.pod.Pohnum0 && prc.Poplin0 == item.pod.Poplin0 && prc.Poqseq0 == item.pod.Poqseq0
                                                        select new
                                                        {
                                                            preciptD = prc,
                                                            stock = all1,
                                                        }).ToListAsync();

                            if (ReciptionLine.Any())
                            {
                                foreach (var itemRc in ReciptionLine)
                                {
                                    var RcMapData = this.mapper.Map<Preceiptd, PurchaseReceiptViewModel>(itemRc.preciptD);
                                    RcMapData.HeatNumber = itemRc?.stock?.Lot0 ?? "";
                                    RcMapData.HeatNumber += itemRc?.stock?.Slo0 ?? "";

                                    var RcDim = await this.repositoryDimLink.GetFirstOrDefaultAsync(x => x, x => x.Vcrnum0 == itemRc.preciptD.Pthnum0 && x.Vcrlin0 == itemRc.preciptD.Ptdlin0);
                                    if (RcDim != null)
                                    {
                                        RcMapData.RcBranch = RcDim.Cce0;
                                        RcMapData.RcWorkItem = RcDim.Cce1;
                                        RcMapData.RcProject = RcDim.Cce2;
                                        RcMapData.RcWorkGroup = RcDim.Cce3;
                                        // Add CCE
                                        ListCCE.Add(RcDim.Cce0);
                                        ListCCE.Add(RcDim.Cce1);
                                        ListCCE.Add(RcDim.Cce2);
                                        ListCCE.Add(RcDim.Cce3);
                                    }
                                    // Add Rc to mapData
                                    MapData.PurchaseReceipts.Add(RcMapData);
                                }
                            }
                            else
                                MapData.DeadLine = MapData.DueDate != null ? MapData.ToDate.Date > MapData.DueDate.Value.Date : false;
                        }
                    }

                    MapDatas.Add(MapData);
                }

                var allDim = await this.repositoryDim.GetToListAsync(x => new { Code = x.Cce0, Desc = x.Des0 }, x => ListCCE.Contains(x.Cce0));
                if (allDim.Any())
                {
                    foreach (var item in MapDatas)
                    {
                        //CCE name purchase request
                        item.BranchName = allDim.FirstOrDefault(x => x.Code == item.Branch)?.Desc ?? "-";
                        item.WorkGroupName = allDim.FirstOrDefault(x => x.Code == item.WorkGroup)?.Desc ?? "-";
                        item.WorkItemName = allDim.FirstOrDefault(x => x.Code == item.WorkItem)?.Desc ?? "-";
                        item.ProjectName = allDim.FirstOrDefault(x => x.Code == item.Project)?.Desc ?? "-";
                        //CCE name purchase order
                        item.PoBranchName = allDim.FirstOrDefault(x => x.Code == item.PoBranch)?.Desc ?? "-";
                        item.PoWorkGroupName = allDim.FirstOrDefault(x => x.Code == item.PoWorkGroup)?.Desc ?? "-";
                        item.PoWorkItemName = allDim.FirstOrDefault(x => x.Code == item.PoWorkItem)?.Desc ?? "-";
                        item.PoProjectName = allDim.FirstOrDefault(x => x.Code == item.PoProject)?.Desc ?? "-";
                        foreach (var item2 in item.PurchaseReceipts)
                        {
                            //CCE name purchase order
                            item2.RcBranchName = allDim.FirstOrDefault(x => x.Code == item2.RcBranch)?.Desc ?? "-";
                            item2.RcWorkGroupName = allDim.FirstOrDefault(x => x.Code == item2.RcWorkGroup)?.Desc ?? "-";
                            item2.RcWorkItemName = allDim.FirstOrDefault(x => x.Code == item2.RcWorkItem)?.Desc ?? "-";
                            item2.RcProjectName = allDim.FirstOrDefault(x => x.Code == item2.RcProject)?.Desc ?? "-";
                        }
                    }
                }

                return new JsonResult(new ScrollDataViewModel<PurchaseRequestAndOrderViewModel>(Scroll, MapDatas), this.DefaultJsonSettings);
            }
            catch (Exception ex)
            {
                Message = $"{ex.ToString()}";
            }
            return BadRequest();
        }

        public async Task<IActionResult> GetScroll2([FromBody] ScrollViewModel Scroll)
        {
            if (Scroll == null)
                return BadRequest();

            // Filter
            var filters = string.IsNullOrEmpty(Scroll.Filter) ? new string[] { "" }
                                : Scroll.Filter.Split(null);

            var predicate = PredicateBuilder.False<Prequisd>();

            foreach (string temp in filters)
            {
                string keyword = temp;
                predicate = predicate.Or(x => FindFunc(x.Pjt0, keyword) ||
                                              FindFunc(x.Itmref0, keyword) ||
                                              FindFunc(x.Itmdes0, keyword) ||
                                              FindFunc(x.Pshnum0, keyword));
            }

            #region FilterLink
            var FilterLink = false;
            var predicateLink = PredicateBuilder.False<Cptanalin>().And(x => x.Abrfic0 == "PSD");
            if (!string.IsNullOrEmpty(Scroll.WhereBranch))
            {
                predicateLink = predicateLink.And(x => x.Cce0 == Scroll.WhereBranch);
                FilterLink = true;
            }
            if (!string.IsNullOrEmpty(Scroll.WhereWorkGroup))
            {
                predicateLink = predicateLink.And(x => x.Cce3 == Scroll.WhereWorkGroup);
                FilterLink = true;
            }
            if (!string.IsNullOrEmpty(Scroll.WhereWorkItem))
            {
                predicateLink = predicateLink.And(x => x.Cce1 == Scroll.WhereWorkItem);
                FilterLink = true;
            }
            if (!string.IsNullOrEmpty(Scroll.WhereProject))
            {
                predicateLink = predicateLink.And(x => x.Cce2 == Scroll.WhereProject);
                FilterLink = true;
            }
            if (FilterLink)
            {
                var ListPr = await this.repositoryDimLink.GetToListAsync(x => new
                {
                    PrNumber = x.Vcrnum0,
                    PrLine = x.Vcrlin0
                }, predicateLink);

                if (ListPr.Any())
                    predicate = predicate.And(x => ListPr.Any(z => z.PrNumber == x.Pqhnum0 && z.PrLine == x.Ppdlin0));
            }
            #endregion

            #region FilterDate

            if (Scroll.SDate.HasValue && Scroll.EDate.HasValue)
            {
                predicate = predicate.And(x =>
                    x.Extrcpdat0.Date >= Scroll.SDate.Value.Date &&
                    x.Extrcpdat0.Date <= Scroll.EDate.Value.Date);
            }

            #endregion

            // Only Item StartWith
            predicate = predicate.And(x => x.Itmref0.StartsWith('1'));

            //Order by
            Func<IQueryable<Prequisd>, IOrderedQueryable<Prequisd>> order;
            // Order
            switch (Scroll.SortField)
            {
                case "PrNumber":
                    if (Scroll.SortOrder == -1)
                        order = o => o.OrderByDescending(x => x.Pshnum0);
                    else
                        order = o => o.OrderBy(x => x.Pshnum0);
                    break;
                case "PrLine":
                    if (Scroll.SortOrder == -1)
                        order = o => o.OrderByDescending(x => x.Psdlin0);
                    else
                        order = o => o.OrderBy(x => x.Psdlin0);
                    break;
                case "Item":
                    if (Scroll.SortOrder == -1)
                        order = o => o.OrderByDescending(x => x.Itmref0);
                    else
                        order = o => o.OrderBy(x => x.Itmref0);
                    break;
                case "ItemName":
                    if (Scroll.SortOrder == -1)
                        order = o => o.OrderByDescending(x => x.Itmdes0);
                    else
                        order = o => o.OrderBy(x => x.Itmdes0);
                    break;
                case "Project":
                    if (Scroll.SortOrder == -1)
                        order = o => o.OrderByDescending(x => x.Pjt0);
                    else
                        order = o => o.OrderBy(x => x.Pjt0);
                    break;
                case "PRDateString":
                    if (Scroll.SortOrder == -1)
                        order = o => o.OrderByDescending(x => x.Extrcpdat0);
                    else
                        order = o => o.OrderBy(x => x.Extrcpdat0);
                    break;
                default:
                    order = o => o.OrderByDescending(x => x.Extrcpdat0);
                    break;
            }

            var QueryData = await this.repository.GetToListAsync(
                                    selector: selected => selected,  // Selected
                                    predicate: predicate, // Where
                                    orderBy: order, // Order
                                    include: null, // Include
                                    skip: Scroll.Skip ?? 0, // Skip
                                    take: Scroll.Take ?? 10); // Take

            // Get TotalRow
            Scroll.TotalRow = await this.repository.GetLengthWithAsync(predicate: predicate);

            var MapDatas = new List<PurchaseRequestAndOrderViewModel>();
            var ListCCE = new List<string>();

            foreach (var item in QueryData)
            {
                var MapData = this.mapper.Map<Prequisd, PurchaseRequestAndOrderViewModel>(item);
                // Get Dimension for PurchaseRequest
                var PrDim = await this.repositoryDimLink.GetFirstOrDefaultAsync(x => x, x => x.Vcrnum0 == item.Pshnum0 && x.Vcrlin0 == item.Psdlin0);
                if (PrDim != null)
                {
                    MapData.Branch = PrDim.Cce0;
                    MapData.WorkItem = PrDim.Cce1;
                    MapData.Project = PrDim.Cce2;
                    MapData.WorkGroup = PrDim.Cce3;
                    // Add CCE
                    ListCCE.Add(PrDim.Cce0);
                    ListCCE.Add(PrDim.Cce1);
                    ListCCE.Add(PrDim.Cce2);
                    ListCCE.Add(PrDim.Cce3);
                }

                // Get Purchase Request Link
                var PrLink = await this.repositoryPRLink.GetFirstOrDefaultAsync(x => x, x => x.Pshnum0 == item.Pshnum0 && x.Psdlin0 == item.Psdlin0);
                if (PrLink != null)
                {
                    this.mapper.Map<Prequiso, PurchaseRequestAndOrderViewModel>(PrLink, MapData);
                    // Get Purchase Order
                    var PoData = await this.repositoryPOLine.GetFirstOrDefaultAsync(x => x, x => x.Pohnum0 == PrLink.Pohnum0 && x.Poplin0 == PrLink.Poplin0 && x.Poqseq0 == PrLink.Poqseq0);
                    if (PoData != null)
                    {
                        this.mapper.Map<Porderq, PurchaseRequestAndOrderViewModel>(PoData, MapData);
                        var PoDim = await this.repositoryDimLink.GetFirstOrDefaultAsync(x => x, x => x.Vcrnum0 == PoData.Pohnum0 && x.Vcrlin0 == PoData.Poplin0 && x.Vcrseq0 == PoData.Poqseq0);
                        if (PoDim != null)
                        {
                            MapData.PoBranch = PoDim.Cce0;
                            MapData.PoWorkItem = PoDim.Cce1;
                            MapData.PoProject = PoDim.Cce2;
                            MapData.PoWorkGroup = PoDim.Cce3;
                            // Add CCE
                            ListCCE.Add(PoDim.Cce0);
                            ListCCE.Add(PoDim.Cce1);
                            ListCCE.Add(PoDim.Cce2);
                            ListCCE.Add(PoDim.Cce3);
                        }

                        var ListRcs = await this.repositoryRCLine.GetToListAsync(
                            x => x,
                            x => x.Pohnum0 == PoData.Pohnum0 && x.Poplin0 == PoData.Poplin0 && x.Poqseq0 == PoData.Poqseq0);
                        if (ListRcs.Any())
                        {
                            foreach (var itemRc in ListRcs)
                            {
                                var RcMapData = this.mapper.Map<Preceiptd, PurchaseReceiptViewModel>(itemRc);
                                var RcDim = await this.repositoryDimLink.GetFirstOrDefaultAsync(x => x, x => x.Vcrnum0 == itemRc.Pthnum0 && x.Vcrlin0 == itemRc.Ptdlin0);
                                if (RcDim != null)
                                {
                                    RcMapData.RcBranch = RcDim.Cce0;
                                    RcMapData.RcWorkItem = RcDim.Cce1;
                                    RcMapData.RcProject = RcDim.Cce2;
                                    RcMapData.RcWorkGroup = RcDim.Cce3;
                                    // Add CCE
                                    ListCCE.Add(RcDim.Cce0);
                                    ListCCE.Add(RcDim.Cce1);
                                    ListCCE.Add(RcDim.Cce2);
                                    ListCCE.Add(RcDim.Cce3);
                                }
                                // Add Rc to mapData
                                MapData.PurchaseReceipts.Add(RcMapData);
                            }
                        }
                    }
                }

                MapDatas.Add(MapData);
            }

            var allDim = await this.repositoryDim.GetToListAsync(x => new { Code = x.Cce0, Desc = x.Des0 }, x => ListCCE.Contains(x.Cce0));
            if (allDim.Any())
            {
                foreach (var item in MapDatas)
                {
                    //CCE name purchase request
                    item.BranchName = allDim.FirstOrDefault(x => x.Code == item.Branch)?.Desc ?? "-";
                    item.WorkGroupName = allDim.FirstOrDefault(x => x.Code == item.WorkGroup)?.Desc ?? "-";
                    item.WorkItemName = allDim.FirstOrDefault(x => x.Code == item.WorkItem)?.Desc ?? "-";
                    item.ProjectName = allDim.FirstOrDefault(x => x.Code == item.Project)?.Desc ?? "-";
                    //CCE name purchase order
                    item.PoBranchName = allDim.FirstOrDefault(x => x.Code == item.PoBranch)?.Desc ?? "-";
                    item.PoWorkGroupName = allDim.FirstOrDefault(x => x.Code == item.PoWorkGroup)?.Desc ?? "-";
                    item.PoWorkItemName = allDim.FirstOrDefault(x => x.Code == item.PoWorkItem)?.Desc ?? "-";
                    item.PoProjectName = allDim.FirstOrDefault(x => x.Code == item.PoProject)?.Desc ?? "-";
                    foreach (var item2 in item.PurchaseReceipts)
                    {
                        //CCE name purchase order
                        item2.RcBranchName = allDim.FirstOrDefault(x => x.Code == item2.RcBranch)?.Desc ?? "-";
                        item2.RcWorkGroupName = allDim.FirstOrDefault(x => x.Code == item2.RcWorkGroup)?.Desc ?? "-";
                        item2.RcWorkItemName = allDim.FirstOrDefault(x => x.Code == item2.RcWorkItem)?.Desc ?? "-";
                        item2.RcProjectName = allDim.FirstOrDefault(x => x.Code == item2.RcProject)?.Desc ?? "-";
                    }
                }
            }

            return new JsonResult(new ScrollDataViewModel<PurchaseRequestAndOrderViewModel>(Scroll, MapDatas), this.DefaultJsonSettings);
        }
    }
}
