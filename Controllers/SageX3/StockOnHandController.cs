﻿using AutoMapper;
using ClosedXML.Excel;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

//3rd Party
using RtfPipe;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VipcoSageX3.Models.SageX3;
using VipcoSageX3.Services;
using VipcoSageX3.ViewModels;

namespace VipcoSageX3.Controllers.SageX3
{
    [Route("api/[controller]")]
    [ApiController]
    public class StockOnHandController : GenericSageX3Controller<Itmmvt>
    {
        // GET: api/StockOnHand
        //Context
        private readonly SageX3Context sageContext;

        private readonly IRepositorySageX3<Stock> repositoryStock;
        private readonly IHostingEnvironment hosting;

        public StockOnHandController(IRepositorySageX3<Itmmvt> repo,
            IRepositorySageX3<Stock> repoStock,
            IHostingEnvironment hosting,
            IMapper mapper, SageX3Context x3Context) : base(repo, mapper)
        {
            // Repoistory
            this.repositoryStock = repoStock;
            // Host
            this.hosting = hosting;
            // Context
            this.sageContext = x3Context;
        }

        #region PrivateMethod
        private async Task<List<StockOnHandViewModel>> GetData(ScrollViewModel Scroll)
        {
            #region Query

            var QueryData = (from ProductsSites in this.sageContext.Itmfacilit
                             join ProductTotal in this.sageContext.Itmmvt on ProductsSites.Itmref0 equals ProductTotal.Itmref0 into ProductStock
                             from nProductStock in ProductStock.DefaultIfEmpty()
                             join ProductMaster in this.sageContext.Itmmaster on ProductsSites.Itmref0 equals ProductMaster.Itmref0 into ProductMaster2
                             from nProductMaster in ProductMaster2.DefaultIfEmpty()
                             join ProductCate in this.sageContext.Itmcateg on nProductMaster.Tclcod0 equals ProductCate.Tclcod0 into ProductCate2
                             from nProductCate in ProductCate2.DefaultIfEmpty()
                             join aText in this.sageContext.Atextra on new { code1 = nProductCate.Tclcod0, code2 = "TCLAXX" } equals new { code1 = aText.Ident10, code2 = aText.Zone0 } into AText
                             from nAText in AText.DefaultIfEmpty()
                             join bText in this.sageContext.Texclob on new { Code0 = nProductMaster.Purtex0 } equals new { bText.Code0 } into bText2
                             from fullText in bText2.DefaultIfEmpty()
                             select new
                             {
                                 ProductsSites,
                                 nProductStock,
                                 nProductMaster,
                                 nProductCate,
                                 nAText,
                                 fullText,
                             }).AsQueryable();

            #endregion Query

            #region Filter

            // Filter
            var filters = string.IsNullOrEmpty(Scroll.Filter) ? new string[] { "" }
                                : Scroll.Filter.Split(null);

            foreach (string temp in filters)
            {
                string keyword = temp.ToLower();
                QueryData = QueryData.Where(x => x.nAText.Texte0.ToLower().Contains(keyword) ||
                                                 x.nProductMaster.Itmdes10.ToLower().Contains(keyword) ||
                                                 x.nProductMaster.Itmdes20.ToLower().Contains(keyword) ||
                                                 x.nProductMaster.Itmdes30.ToLower().Contains(keyword) ||
                                                 x.ProductsSites.Itmref0.ToLower().Contains(keyword));
            }

            // Product Category
            if (Scroll.WhereBanks.Any())
                QueryData = QueryData.Where(x => Scroll.WhereBanks.Contains(x.nProductMaster.Tclcod0));

            QueryData = QueryData.Where(x => x.nProductStock.Physto0 > 0 || x.ProductsSites.Ofs0 > 0);

            #endregion Filter

            #region Scroll

            switch (Scroll.SortField)
            {
                case "ItemCode":
                    if (Scroll.SortOrder == -1)
                        QueryData = QueryData.OrderByDescending(x => x.nProductMaster.Itmref0);
                    else
                        QueryData = QueryData.OrderBy(x => x.nProductMaster.Itmref0);
                    break;

                case "ItemDescFull":
                    if (Scroll.SortOrder == -1)
                        QueryData = QueryData.OrderByDescending(x => x.nProductMaster.Itmdes10);
                    else
                        QueryData = QueryData.OrderBy(x => x.nProductMaster.Itmdes10);
                    break;

                case "Uom":
                    if (Scroll.SortOrder == -1)
                        QueryData = QueryData.OrderByDescending(x => x.nProductMaster.Stu0);
                    else
                        QueryData = QueryData.OrderBy(x => x.nProductMaster.Stu0);
                    break;

                case "Category":
                    if (Scroll.SortOrder == -1)
                        QueryData = QueryData.OrderByDescending(x => x.nProductCate.Tclcod0);
                    else
                        QueryData = QueryData.OrderBy(x => x.nProductCate.Tclcod0);
                    break;

                case "CategoryDesc":
                    if (Scroll.SortOrder == -1)
                        QueryData = QueryData.OrderByDescending(x => x.nAText.Texte0);
                    else
                        QueryData = QueryData.OrderBy(x => x.nAText.Texte0);
                    break;

                case "InternelStockString":
                    if (Scroll.SortOrder == -1)
                        QueryData = QueryData.OrderByDescending(x => x.nProductStock.Physto0);
                    else
                        QueryData = QueryData.OrderBy(x => x.nProductStock.Physto0);
                    break;

                case "OnOrderString":
                    if (Scroll.SortOrder == -1)
                        QueryData = QueryData.OrderByDescending(x => x.ProductsSites.Ofs0);
                    else
                        QueryData = QueryData.OrderBy(x => x.ProductsSites.Ofs0);
                    break;

                default:
                    QueryData = QueryData.OrderByDescending(x => x.nProductMaster.Itmref0);
                    break;
            }

            #endregion Scroll

            Scroll.TotalRow = await QueryData.CountAsync();
            var Message = "";
            try
            {
                var Datasource = await QueryData.Skip(Scroll.Skip ?? 0).Take(Scroll.Take ?? 15).AsNoTracking().ToListAsync();
                var MapDatas = new List<StockOnHandViewModel>();

                foreach (var item in Datasource)
                {
                    var MapData = new StockOnHandViewModel()
                    {
                        Category = item?.nProductCate?.Tclcod0,
                        CategoryDesc = item?.nAText?.Texte0,
                        InternelStock = (double)(item?.nProductStock?.Physto0 ?? 0),
                        ItemCode = item?.nProductMaster?.Itmref0,
                        ItemDesc = item?.nProductMaster?.Itmdes10,
                        OnOrder = (double)(item?.ProductsSites?.Ofs0 ?? 0),
                        Uom = item?.nProductMaster?.Stu0,
                    };

                    // Set Stock
                    var ListStock = await this.repositoryStock.GetToListAsync(x => x, x => x.Itmref0 == MapData.ItemCode);
                    if (ListStock != null && ListStock.Any())
                    {
                        foreach (var stock in ListStock.GroupBy(x => x.Loc0))
                        {
                            MapData.StockLocations.Add(new StockLocationViewModel
                            {
                                LocationCode = stock.Key,
                                Quantity = (double)(stock?.Sum(z => z.Qtypcu0) ?? (decimal)0)
                            });
                        }
                    }

                    //ItemName
                    if (item.fullText?.Texte0 != null)
                    {
                        if (item.fullText.Texte0.StartsWith("{\\rtf1"))
                            MapData.ItemDescFull = Rtf.ToHtml(item.fullText?.Texte0);
                        else
                            MapData.ItemDescFull = item?.fullText?.Texte0;
                    }
                    else
                        MapData.ItemDescFull = item?.fullText?.Texte0 ?? "-";

                    MapDatas.Add(MapData);
                }

                return MapDatas;
            }
            catch (Exception ex)
            {
                Message = $"{ex.ToString()}";
            }

            return null;
        }
        private string ConvertHtmlToText(string Html)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(Html);

            var htmlBody = htmlDoc.DocumentNode;
            // return htmlBody.OuterHtml;
            var sb = new StringBuilder();
            foreach (var node in htmlBody.DescendantsAndSelf())
            {
                if (!node.HasChildNodes)
                {
                    string text = node.InnerText;
                    text = text.Replace("&nbsp;", "");

                    if (!string.IsNullOrEmpty(text))
                        sb.AppendLine(text.Trim());
                }
            }
            return sb.ToString();
        }
        #endregion

        // POST: api/StockOnHand/GetScroll
        [HttpPost("GetScroll")]
        public async Task<IActionResult> GetScroll([FromBody] ScrollViewModel Scroll)
        {
            if (Scroll == null)
                return BadRequest();

            var Message = "";

            try
            {
                var HasData = await this.GetData(Scroll);
                return new JsonResult(new ScrollDataViewModel<StockOnHandViewModel>(Scroll, HasData), this.DefaultJsonSettings);
            }
            catch (Exception ex)
            {
                Message = $"{ex.ToString()}";
            }
            return BadRequest(new { Error = Message });
        }
        // POST: api/StockOnHand/GetReport/
        [HttpPost("GetReport")]
        public async Task<IActionResult> GetReport([FromBody] ScrollViewModel Scroll)
        {
            if (Scroll == null)
                return BadRequest();
            var Message = "";
            var MapDatas = await this.GetData(Scroll);
            try
            {
                if (MapDatas.Any())
                {
                    var table = new DataTable();
                    //Adding the Columns
                    table.Columns.AddRange(new DataColumn[]
                    {
                        new DataColumn("Item Code", typeof(string)),
                        new DataColumn("Item Desc.", typeof(string)),
                        new DataColumn("Category Code",typeof(string)),
                        new DataColumn("Category",typeof(string)),
                        new DataColumn("Stock",typeof(string)),
                        new DataColumn("OnOrder",typeof(string)),
                        new DataColumn("Uom",typeof(string)),
                    });

                    //Adding the Rows
                    foreach (var item in MapDatas)
                    {
                        item.ItemDesc = this.ConvertHtmlToText(item.ItemDescFull);

                        table.Rows.Add(
                            item.ItemCode,
                            item.ItemDesc,
                            item.Category,
                            item.CategoryDesc,
                            item.InternelStockString,
                            item.OnOrderString,
                            item.Uom
                        );
                    }

                    var templateFolder = this.hosting.WebRootPath + "/reports/";
                    var fileExcel = templateFolder + $"StockOnHand_Report.xlsx";

                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        var wsFreeze = wb.Worksheets.Add(table, "StockOnHandReport");
                        wsFreeze.Columns().AdjustToContents();
                        wsFreeze.SheetView.FreezeRows(1);
                        wb.SaveAs(fileExcel);
                    }

                    var memory = new MemoryStream();
                    using (var stream = new FileStream(fileExcel, FileMode.Open))
                    {
                        await stream.CopyToAsync(memory);
                    }
                    memory.Position = 0;

                    return File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Payment_Report.xlsx");
                }
            }
            catch (Exception ex)
            {
                Message = $"{ex.ToString()}";
            }

            return BadRequest();
        }
    }
}