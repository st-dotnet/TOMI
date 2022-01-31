using TOMI.Data.Database.Entities;

namespace TOMI.Services.Models
{
    public class MasterDataResponse
    {
        public string SKU { get; set; }
    
        public string Barcode { get; set; }
        
        public string RetailPrice { get; set; }
       
        public string Description { get; set; }

        public string Department { get; set; }
      
        public string Blank { get; set; }
       
        public string OHQuantity { get; set; }
      
        public string Unity { get; set; }

    }

    public class MasterResponse:BaseResponse
    {
        public Master master { get; set; }
    }
}
