using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCBikesWebpage.Models
{
    public class BikeListModel : DBEntities
    {
        public BikeListModel(string ProdModel, string Desc, int ProdModelID)
        {
            ProductModel = ProdModel;
            Description = Desc;
            ProductModelID = ProdModelID;
        }

        public string ProductModel { get; set; }
        public string Description { get; set; }
        public int ProductModelID { get; set; }
    }
}