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
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : GenericSageX3Controller<Paymenth>
    {
        private readonly IRepositorySageX3<Bank> repositoryBank;
        public PaymentController(IRepositorySageX3<Paymenth> repo,
            IRepositorySageX3<Bank> repoBank,
            IMapper mapper) : base(repo, mapper)
        {
            this.repositoryBank = repoBank;
        }

        // POST: api/Payment/GetScroll
        [HttpPost("GetScroll")]
        public async Task<IActionResult> GetScroll([FromBody] ScrollViewModel Scroll)
        {
            if (Scroll == null)
                return BadRequest();


            // Filter
            var filters = string.IsNullOrEmpty(Scroll.Filter) ? new string[] { "" }
                                : Scroll.Filter.Split(null);

            var predicate = PredicateBuilder.False<Paymenth>();

            foreach (string temp in filters)
            {
                string keyword = temp;
                predicate = predicate.Or(x => FindFunc(x.Chqnum0, keyword) ||
                                              FindFunc(x.Cur0, keyword) ||
                                              FindFunc(x.Des0, keyword) ||
                                              FindFunc(x.Bpr0, keyword) ||
                                              FindFunc(x.Num0, keyword) ||
                                              FindFunc(x.Ref0, keyword) ||
                                              FindFunc(x.Bpainv0, keyword) ||
                                              FindFunc(x.Bpanam0, keyword));
            }
            // Bank
            if (!string.IsNullOrEmpty(Scroll.WhereBank))
                predicate = predicate.And(x => x.Ban0 == Scroll.WhereBank);
            // Supplier
            if (!string.IsNullOrEmpty(Scroll.WhereSupplier))
                predicate = predicate.And(x => x.Bpr0 == Scroll.WhereSupplier);
            //Order by
            Func<IQueryable<Paymenth>, IOrderedQueryable<Paymenth>> order;
            // Order
            switch (Scroll.SortField)
            {
                case "AmountString":
                    if (Scroll.SortOrder == -1)
                        order = o => o.OrderByDescending(x => x.Amtcur0);
                    else
                        order = o => o.OrderBy(x => x.Amtcur0);
                    break;
                case "BankNo":
                    if (Scroll.SortOrder == -1)
                        order = o => o.OrderByDescending(x => x.Ban0);
                    else
                        order = o => o.OrderBy(x => x.Ban0);
                    break;
                case "CheckNo":
                    if (Scroll.SortOrder == -1)
                        order = o => o.OrderByDescending(x => x.Chqnum0);
                    else
                        order = o => o.OrderBy(x => x.Chqnum0);
                    break;
                case "Currency":
                    if (Scroll.SortOrder == -1)
                        order = o => o.OrderByDescending(x => x.Cur0);
                    else
                        order = o => o.OrderBy(x => x.Cur0);
                    break;
                case "Description":
                    if (Scroll.SortOrder == -1)
                        order = o => o.OrderByDescending(x => x.Des0);
                    else
                        order = o => o.OrderBy(x => x.Des0);
                    break;
                case "PayBy":
                    if (Scroll.SortOrder == -1)
                        order = o => o.OrderByDescending(x => x.Bpr0);
                    else
                        order = o => o.OrderBy(x => x.Bpr0);
                    break;
                case "PaymentDate":
                    if (Scroll.SortOrder == -1)
                        order = o => o.OrderByDescending(x => x.Accdat0);
                    else
                        order = o => o.OrderBy(x => x.Accdat0);
                    break;
                case "PaymentNo":
                    if (Scroll.SortOrder == -1)
                        order = o => o.OrderByDescending(x => x.Num0);
                    else
                        order = o => o.OrderBy(x => x.Num0);
                    break;
                case "SupplierNo":
                    if (Scroll.SortOrder == -1)
                        order = o => o.OrderByDescending(x => x.Bpainv0);
                    else
                        order = o => o.OrderBy(x => x.Bpainv0);
                    break;
                case "SupplierName":
                    if (Scroll.SortOrder == -1)
                        order = o => o.OrderByDescending(x => x.Bpanam0);
                    else
                        order = o => o.OrderBy(x => x.Bpanam0);
                    break;
                default:
                    order = o => o.OrderByDescending(x => x.Accdat0);
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

            var mapDatas = new List<PaymentViewModel>();
            foreach (var item in QueryData)
            {
                var MapItem = this.mapper.Map<Paymenth, PaymentViewModel>(item);
                //Project Name
                if (!string.IsNullOrEmpty(item.Ban0))
                {
                    MapItem.BankName += await this.repositoryBank.GetFirstOrDefaultAsync(z => z.Des0, z => z.Ban0 == item.Ban0) ?? "-";
                }
                mapDatas.Add(MapItem);
            }

            return new JsonResult(new ScrollDataViewModel<PaymentViewModel>(Scroll, mapDatas), this.DefaultJsonSettings);
        }
    }
}
