using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using MyBlazorWasmApp.Models;

// using System.Net.Http; remove .json when you connect to db

namespace MyBlazorWasmApp.Pages.components.Dashboard
{
    public partial class Dashboard
    {
        // Enum for sorting column
        public enum SortColumn
        {
            Url,
            IsHealthy,
            PercentageChange,
            SignificanceChange,
            LastScraped,
        }

        // List of URL data
        private List<UrlData> urlDataList = new List<UrlData>();
        private List<UrlData> filteredData = new List<UrlData>();
        private List<UrlData> pagedData = new List<UrlData>();
        private DashboardData dashboardData = new DashboardData();
        private Cards cards = new Cards();
        private string searchTerm = "";
        private string healthFilter = "";
        private int currentPage = 1;
        private int pageSize = 10;
        private int totalPages = 1;

        // Sorting variables
        private SortColumn sortColumn = SortColumn.Url; // Default sort column
        private bool sortAscending = true;

        // Summary Calculations
        private int healthyUrls => filteredData.Count(u => u.IsHealthy);
        private int brokenUrls => filteredData.Count(u => !u.IsHealthy);
        private int significantChangeUrls => filteredData.Count(u => u.SignificanceChange > 20);
        private int urlsWithChanges => filteredData.Count(u => u.PercentageChange != 0);

        // Load the initial data asynchronously

        [Inject]
        public HttpClient Http { get; set; }

        protected override async Task OnInitializedAsync()
        {
            urlDataList = await Http.GetFromJsonAsync<List<UrlData>>(
                "sample-data/dummy-url-data.json"
            );

            dashboardData = await Http.GetFromJsonAsync<DashboardData>(
                "sample-data/dashboard-data.json"
            );

            cards = dashboardData?.cards;

            ApplyFilters();
            UpdatePagedData();
        }

        // Apply filters based on search term and health status
        private void ApplyFilters()
        {
            filteredData = urlDataList
                .Where(u =>
                    string.IsNullOrEmpty(searchTerm)
                    || u.Url.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                )
                .Where(u =>
                    string.IsNullOrEmpty(healthFilter)
                    || (healthFilter == "Healthy" && u.IsHealthy)
                    || (healthFilter == "Unhealthy" && !u.IsHealthy)
                )
                .ToList();

            // Reset the current page to 1 whenever filters are applied
            currentPage = 1;

            totalPages = (int)Math.Ceiling((double)filteredData.Count / pageSize);
            UpdatePagedData();
        }

        // Filter by Total URLs with Changes
        private void FilterByUrlsWithChanges()
        {
            filteredData = urlDataList.Where(u => u.PercentageChange != 0).ToList();
            ApplyFilters(); // Reapply filters and refresh the page
        }

        // Filter by Healthy URLs
        private void FilterByHealthyUrls()
        {
            filteredData = urlDataList.Where(u => u.IsHealthy).ToList();
            ApplyFilters();
            UpdatePagedData();
        }

        // Filter by Broken URLs
        private void FilterByBrokenUrls()
        {
            filteredData = urlDataList.Where(u => !u.IsHealthy).ToList();
            ApplyFilters();
            UpdatePagedData();
        }

        // Filter by Significant Changes
        private void FilterBySignificantChanges()
        {
            filteredData = urlDataList.Where(u => u.SignificanceChange > 20).ToList();
            ApplyFilters();
            UpdatePagedData();
        }

        // Sorting logic for different columns using the enum
        private void SortBy(SortColumn column)
        {
            if (sortColumn == column)
            {
                sortAscending = !sortAscending; // Toggle sorting order
            }
            else
            {
                sortColumn = column;
                sortAscending = true; // Reset to ascending order when switching columns
            }

            // Sort based on the selected column and order
            filteredData = sortColumn switch
            {
                SortColumn.Url => sortAscending
                    ? filteredData.OrderBy(u => u.Url).ToList()
                    : filteredData.OrderByDescending(u => u.Url).ToList(),
                SortColumn.IsHealthy => sortAscending
                    ? filteredData.OrderBy(u => u.IsHealthy).ToList()
                    : filteredData.OrderByDescending(u => u.IsHealthy).ToList(),
                SortColumn.PercentageChange => sortAscending
                    ? filteredData.OrderBy(u => u.PercentageChange).ToList()
                    : filteredData.OrderByDescending(u => u.PercentageChange).ToList(),
                SortColumn.SignificanceChange => sortAscending
                    ? filteredData.OrderBy(u => u.SignificanceChange).ToList()
                    : filteredData.OrderByDescending(u => u.SignificanceChange).ToList(),
                SortColumn.LastScraped => sortAscending
                    ? filteredData.OrderBy(u => u.LastScraped).ToList()
                    : filteredData.OrderByDescending(u => u.LastScraped).ToList(),
                _ => filteredData,
            };

            // Update the paginated data after sorting
            UpdatePagedData();
        }

        // Next page pagination logic
        private void NextPage()
        {
            if (currentPage < totalPages)
            {
                currentPage++;
                UpdatePagedData();
            }
        }

        // Previous page pagination logic
        private void PreviousPage()
        {
            if (currentPage > 1)
            {
                currentPage--;
                UpdatePagedData();
            }
        }

        // Update the paged data based on current page and page size
        private void UpdatePagedData()
        {
            // Ensure the current page is within bounds
            currentPage = Math.Min(currentPage, totalPages);

            pagedData = filteredData.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
        }
    }
}
