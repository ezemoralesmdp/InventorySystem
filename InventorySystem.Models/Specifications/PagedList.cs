namespace InventorySystem.Models.Specifications
{
    public class PagedList<T> : List<T>
    {
        public MetaData MetaData { get; set; }

        public PagedList(List<T> items, int count, int pageNumber, int pageSize)
        {
            MetaData = new()
            {
                TotalCount = count,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(count / (double)pageSize) //Examnple: 1.5 rounded to 2
            };

            AddRange(items); //Elements has been added to the end of the list
        }

        public static PagedList<T> ToPagedList(IEnumerable<T> entity, int pageNumber, int pageSize)
        {
            var count = entity.Count();
            var items = entity.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            return new PagedList<T>(items, count, pageNumber, pageSize);
        }
    }
}