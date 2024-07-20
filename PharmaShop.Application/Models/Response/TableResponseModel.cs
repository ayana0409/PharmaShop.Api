namespace PharmaShop.Application.Models.Response
{
    public class TableResponseModel<T> where T : class
    {
        public int PageSize { get; set; }
        public IEnumerable<T> Datas { get; set; } = [];
        public int Total { get; set; }
    }
}
