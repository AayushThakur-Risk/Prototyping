namespace MyBlazorWasmApp.Models
{
    public class DashboardData
    {
        public List<Cards> cards { get; set; }
        public List<MapData> mapData { get; set; }
        public TablePagination tablePagination { get; set; }
        public List<TableData> tableData { get; set; }
        public List<ColumnLabels> columnLabels { get; set; }
    }

    public class Cards
    {
        public string label { get; set; }
        public string heading { get; set; }
        public double count { get; set; }
        public string description { get; set; }
        public string cssClass { get; set; }
    }

    public class MapData
    {
        public string state { get; set; }
        public double totalUrls { get; set; }
        public double errors { get; set; }
        public double offline { get; set; }
        public double online { get; set; }
    }

    public class TablePagination
    {
        public int totalRows { get; set; }
        public int rowsPerPage { get; set; }
        public int currentPage { get; set; }
    }

    public class TableData
    {
        public string url { get; set; }
        public string state { get; set; }
        public string status { get; set; }
        public string priority { get; set; }
        public string lastCheckStatus { get; set; }
        public DateTime lastUpdated { get; set; }
    }

    public class SelectOption
    {
        public string Value { get; set; }
        public string Text { get; set; }
    }

    public class ColumnLabels
    {
        public string title { get; set; }
        public string key { get; set; }
    }
}
