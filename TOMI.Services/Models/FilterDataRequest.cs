using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using TOMI.Data.Database.Entities;

namespace TOMI.Services.Models
{
    public class FilterDataRequest
    {
        public Guid? CustomerId { get; set; }
        public Guid? StoreId { get; set; }
        public DateTime? StockDate { get; set; }
    }

    public class FilterDataModel
    {

        public IFormFile File { get; set; }
      
        public Guid? CustomerId { get; set; }
        public Guid? StoreId { get; set; }
        public DateTime? StockDate { get; set; }
        public string StoreName { get; set; }
    }
    public class TerminalModel
    {
        public Guid? CustomerId { get; set; }
        public Guid? StoreId { get; set; }
        public DateTime? Date { get; set; }
        public int CountType { get; set; }
        public string InventaryKey { get; set; }
    }

    public class GetTerminalModel
    {

        public int CountType { get; set; }
        public string InventaryKey { get; set; }

    }


    public class EmpDataResponse : BaseResponse
    {
        public Employee Employee { get; set; }
    }
    public class TerminalPost
    {
        public int Tag { get; set; }
        public int? Shelf { get; set; }
        public int? Quantity { get; set; }
        public string Code { get; set; }
        public List<ShelfList> ShelfList { get; set; }
    }



    public class TerminalDataModels
    {
        public Guid CustomerId { get; set; }
        public Guid StoreId { get; set; }
        public int Tag { get; set; }
        public List<Shelves> Shelves { get; set; }
    }



    public class TerminalDataResponse
    {
        public Guid CustomerId { get; set; }
        public Guid StoreId { get; set; }
        public int Tag { get; set; }
        public List<Shelves> Shelves { get; set; }
    }



    public class Shelves
    {
        public int? Shelf { get; set; }
        public List<Products> products { get; set; }
    }

    public class Products
    {
        public string Code { get; set; }
        public string Department { get; set; }
        public int total_counted { get; set; }
        public DateTimeOffset? Inventory_Date { get; set; }
    }

    public class ShelfList
    {
        public int? Shelf { get; set; }
        public int? Quantity { get; set; }
        public string Code { get; set; }
    }


    public class Taglist
    {
        public int Tag { get; set; }

    }

    public class TerminalPostResponse
    {
        public List<Taglist> Taglist { get; set; }
        public List<ShelfList> ShelfList { get; set; }

    }
    public class MF2Model
    {
        public Guid? CustomerId { get; set; }
        public Guid? StoreId { get; set; }
        public DateTime? Date { get; set; }
        public string Department { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
    }

    public class TerminalResponse : BaseResponse
    {
        public MF1 MF1 { get; set; }
    }

    public class TerminalMF2Response : BaseResponse
    {
        public MF2 MF2 { get; set; }
    }
}
