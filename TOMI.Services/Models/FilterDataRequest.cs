using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
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
        public string Terminal { get; set; }
        public string Employee_Number { get; set; }
        public DateTimeOffset? Inventory_Date { get; set; }
        public int tag { get; set; }
        public int shelf { get; set; }
        public int operation { get; set; }
        public DateTime creation_time { get; set; }
        public string inventory_key { get; set; }
        public bool? sync_to_terminal_status { get; set; }
        public DateTime? sync_to_terminal_time { get; set; }
        public bool? sync_back_from_terminal_status { get; set; }
        public DateTime? sync_back_from_terminal_time { get; set; }
        public int count_type { get; set; }
        public int total_counted { get; set; }
        public DateTime count_time { get; set; }
        public bool nof { get; set; }
        public bool counted_status { get; set; }
        public int CountType { get; set; }
        public string InventaryKey { get; set; }
    }

    public class GetTerminalModel
    {

        public int operation { get; set; }
        public string InventoryKey { get; set; }

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
        public string Tag { get; set; }
        public List<Shelves> Shelves { get; set; }
    }

    public class TerminalDataModelsResponse 
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public HttpStatusCode Status { get; set; }
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
        public string Shelf { get; set; }
        public List<Products> Products { get; set; }
    }

    public class Products
    {
        public string Code { get; set; }
        public string Department { get; set; }
        public int TotalCounted { get; set; }
        public DateTimeOffset? InventoryDate { get; set; }
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
        public Terminal_Smf MF1 { get; set; }
    }

    public class TerminalMF2Response : BaseResponse
    {
        public Terminal_Department MF2 { get; set; }
    }
}
