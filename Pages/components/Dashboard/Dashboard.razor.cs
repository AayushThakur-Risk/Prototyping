using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
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
            url,
            state,
            status,
            priority,
            lastCheckStatus,
            lastUpdated,
        }

        // List of URL data
        private List<UrlData> urlDataList = new List<UrlData>();
        private List<TableData> filteredData = new List<TableData>();
        private List<TableData> pagedData = new List<TableData>();
        private DashboardData dashboardData = new DashboardData();
        private string searchTerm = "";
        private string healthFilter = "";
        private int currentPage = 1;
        private int pageSize = 10;
        private int totalPages = 1;

        // Sorting variables
        private SortColumn sortColumn = SortColumn.url; // Default sort column
        private bool sortAscending = true;

        // Summary Calculations
        // private int healthyUrls => filteredData.Count(u => u.IsHealthy);
        // private int brokenUrls => filteredData.Count(u => !u.IsHealthy);
        // private int significantChangeUrls => filteredData.Count(u => u.SignificanceChange > 20);
        // private int urlsWithChanges => filteredData.Count(u => u.PercentageChange != 0);

        // Load the initial data asynchronously

        [Inject]
        public HttpClient Http { get; set; }

        [Parameter]
        public EventCallback<string> OnSearchChanged { get; set; }
        private string SearchText { get; set; } = string.Empty;

        private List<SelectOption> StateOptions = new()
        {
            new SelectOption { Value = "all", Text = "All States" },
            new SelectOption { Value = "alabama", Text = "Alabama" },
            new SelectOption { Value = "alaska", Text = "Alaska" },
            new SelectOption { Value = "arizona", Text = "Arizona" },
            new SelectOption { Value = "arkansas", Text = "Arkansas" },
            new SelectOption { Value = "california", Text = "California" },
            new SelectOption { Value = "colorado", Text = "Colorado" },
            new SelectOption { Value = "connecticut", Text = "Connecticut" },
            new SelectOption { Value = "delaware", Text = "Delaware" },
            new SelectOption { Value = "florida", Text = "Florida" },
            new SelectOption { Value = "georgia", Text = "Georgia" },
            new SelectOption { Value = "hawaii", Text = "Hawaii" },
        };

        [Parameter]
        public EventCallback<string> OnOptionSelected { get; set; }

        private bool IsMapView = true; // Default view is Map View

        protected override async Task OnInitializedAsync()
        {
            urlDataList = await Http.GetFromJsonAsync<List<UrlData>>(
                "sample-data/dummy-url-data.json"
            );

            dashboardData = await Http.GetFromJsonAsync<DashboardData>(
                "sample-data/dashboard-data.json"
            );

            StateOptions = await Http.GetFromJsonAsync<List<SelectOption>>(
                "sample-data/states.json"
            );

            // var json = await Http.GetStringAsync("states.json");
            // StateOptions = System.Text.Json.JsonSerializer.Deserialize<List<SelectOption>>(json);

            // ApplyFilters();
            UpdatePagedData();
        }

        private void SetView(bool isMapView)
        {
            IsMapView = isMapView;
        }

        private async Task HandleChange(ChangeEventArgs e)
        {
            var selectedValue = e.Value?.ToString();
            if (OnOptionSelected.HasDelegate)
            {
                await OnOptionSelected.InvokeAsync(selectedValue);
            }
        }

        private async Task HandleSearchChanged()
        {
            if (OnSearchChanged.HasDelegate)
            {
                await OnSearchChanged.InvokeAsync(SearchText);
            }
        }

        // Apply filters based on search term and health status
        private void ApplyFilters()
        {
            filteredData = dashboardData?.tableData;
            // .Where(u =>
            //     string.IsNullOrEmpty(searchTerm)
            //     || u.url.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
            // )
            // .Where(u =>
            //     string.IsNullOrEmpty(healthFilter)
            //     // || (healthFilter == "Healthy" && u.IsHealthy)
            //     // || (healthFilter == "Unhealthy" && !u.IsHealthy)
            //     || (healthFilter == "Unhealthy" && !u.IsHealthy)
            // )
            // .ToList();

            // Reset the current page to 1 whenever filters are applied
            currentPage = 1;

            totalPages = (int)Math.Ceiling((double)filteredData.Count / pageSize);
            UpdatePagedData();
        }

        // Filter by Total URLs with Changes
        private void FilterByUrlsWithChanges()
        {
            // filteredData = urlDataList.Where(u => u.PercentageChange != 0).ToList();
            ApplyFilters(); // Reapply filters and refresh the page
        }

        // Filter by Healthy URLs
        // private void FilterByHealthyUrls()
        // {
        //     filteredData = urlDataList.Where(u => u.IsHealthy).ToList();
        //     ApplyFilters();
        //     UpdatePagedData();
        // }

        // Filter by Broken URLs
        // private void FilterByBrokenUrls()
        // {
        //     filteredData = urlDataList.Where(u => !u.IsHealthy).ToList();
        //     ApplyFilters();
        //     UpdatePagedData();
        // }

        // Filter by Significant Changes
        // private void FilterBySignificantChanges()
        // {
        //     filteredData = urlDataList.Where(u => u.SignificanceChange > 20).ToList();
        //     ApplyFilters();
        //     UpdatePagedData();
        // }

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
                SortColumn.url => sortAscending
                    ? filteredData.OrderBy(u => u.url).ToList()
                    : filteredData.OrderByDescending(u => u.url).ToList(),
                SortColumn.state => sortAscending
                    ? filteredData.OrderBy(u => u.state).ToList()
                    : filteredData.OrderByDescending(u => u.state).ToList(),
                SortColumn.status => sortAscending
                    ? filteredData.OrderBy(u => u.status).ToList()
                    : filteredData.OrderByDescending(u => u.status).ToList(),
                SortColumn.priority => sortAscending
                    ? filteredData.OrderBy(u => u.priority).ToList()
                    : filteredData.OrderByDescending(u => u.priority).ToList(),
                SortColumn.lastCheckStatus => sortAscending
                    ? filteredData.OrderBy(u => u.lastCheckStatus).ToList()
                    : filteredData.OrderByDescending(u => u.lastCheckStatus).ToList(),
                SortColumn.lastUpdated => sortAscending
                    ? filteredData.OrderBy(u => u.lastUpdated).ToList()
                    : filteredData.OrderByDescending(u => u.lastUpdated).ToList(),
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
