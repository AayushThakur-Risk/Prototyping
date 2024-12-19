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
        // List of URL data
        // private List<TableData> filteredData = new List<TableData>();
        private List<ColumnLabels> columnLabels = new List<ColumnLabels>();
        private List<TableData> pagedData = new List<TableData>();
        private DashboardData dashboardData = new DashboardData();
        private TablePagination tablePagination = new TablePagination();

        private List<object> FilteredData =>
            string.IsNullOrWhiteSpace(SearchText)
                ? dashboardData?.tableData.Cast<object>().ToList()
                : dashboardData
                    ?.tableData.Where(data => FilterCondition(data))
                    .Cast<object>()
                    .ToList();

        private string searchTerm = "";
        private string healthFilter = "";
        private int currentPage = 1;
        private int pageSize = 10;
        private int totalPages = 1;
        private string currentSortKey = ""; // Current column key being sorted
        private bool isAscending = true; // Sort direction (ascending or descending)

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

        private int TotalPages =>
            (int)Math.Ceiling((double)tablePagination.totalRows / tablePagination.rowsPerPage);

        [Parameter]
        public EventCallback<string> OnOptionSelected { get; set; }

        private bool IsMapView = true; // Default view is Map View
        private string SelectedDomain = "traffic"; // Default value

        private Timer? _debounceTimer;

        protected override async Task OnInitializedAsync()
        {
            StateOptions = await Http.GetFromJsonAsync<List<SelectOption>>(
                "sample-data/states.json"
            );

            // ApplyFilters();
            // Load the default JSON file on component initialization
            await LoadJsonDataAsync();
            // UpdatePagedData();
        }

        private async Task OnDomainChanged(ChangeEventArgs e)
        {
            SelectedDomain = e.Value?.ToString() ?? "";
            Console.WriteLine($"Selected domain changed to: {SelectedDomain}");
            await LoadJsonDataAsync();
        }

        private void OnSearchInput(ChangeEventArgs e)
        {
            SearchText = e.Value?.ToString() ?? string.Empty;

            _debounceTimer?.Dispose();
            _debounceTimer = new Timer(
                _ =>
                {
                    InvokeAsync(StateHasChanged);
                },
                null,
                300,
                Timeout.Infinite
            );
        }

        private async Task LoadJsonDataAsync()
        {
            try
            {
                // Determine the file to load based on the selected domain
                string fileName = SelectedDomain == "traffic" ? "traffic.json" : "student.json";

                // Fetch the JSON data
                dashboardData = await Http.GetFromJsonAsync<DashboardData>(
                    "sample-data/" + SelectedDomain + ".json"
                );

                tablePagination = dashboardData.tablePagination;

                tablePagination.totalRows = dashboardData.tableData?.Count ?? 0;

                columnLabels = dashboardData.columnLabels;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading data: {ex.Message}");
            }
        }

        private void SetView(bool isMapView)
        {
            IsMapView = isMapView;
        }

        private async Task HandleChange(ChangeEventArgs e)
        {
            var selectedValue = e.Value?.ToString();
            Console.WriteLine(selectedValue);
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
            // filteredData = dashboardData?.tableData;
            // // Reset the current page to 1 whenever filters are applied
            // currentPage = 1;

            // totalPages = (int)Math.Ceiling((double)filteredData.Count / pageSize);
            // UpdatePagedData();
        }

        // Filter by Total URLs with Changes
        private void FilterByUrlsWithChanges()
        {
            // filteredData = urlDataList.Where(u => u.PercentageChange != 0).ToList();
            // ApplyFilters(); // Reapply filters and refresh the page
        }

        private void SortBy(string key)
        {
            if (currentSortKey == key)
            {
                // Toggle sort direction if sorting the same column
                isAscending = !isAscending;
            }
            else
            {
                // Set new sort column and default to ascending
                currentSortKey = key;
                isAscending = true;
            }

            // Sort the data
            if (dashboardData?.tableData != null)
            {
                if (isAscending)
                {
                    dashboardData.tableData = dashboardData
                        .tableData.OrderBy(data => GetPropertyValue(data, key)?.ToString())
                        .ToList();
                }
                else
                {
                    dashboardData.tableData = dashboardData
                        .tableData.OrderByDescending(data =>
                            GetPropertyValue(data, key)?.ToString()
                        )
                        .ToList();
                }
            }
        }

        private object GetPropertyValue(object obj, string propertyName)
        {
            if (obj == null || string.IsNullOrEmpty(propertyName))
                return null;

            var property = obj.GetType().GetProperty(propertyName);
            return property?.GetValue(obj);
        }

        private bool FilterCondition(object data)
        {
            foreach (var column in dashboardData?.columnLabels ?? new List<ColumnLabels>())
            {
                var value = GetPropertyValue(data, column.key)?.ToString();
                if (
                    !string.IsNullOrWhiteSpace(value)
                    && value.Contains(SearchText, StringComparison.OrdinalIgnoreCase)
                )
                {
                    Console.WriteLine($"Value: {value}");
                    Console.WriteLine("true");
                    return true;
                }
            }
            Console.WriteLine("false");
            return false;
        }

        private List<MyBlazorWasmApp.Models.TableData> GetFilteredAndPaginatedData()
        {
            // Apply filtering
            var filteredData = string.IsNullOrWhiteSpace(SearchText)
                ? dashboardData?.tableData
                : dashboardData?.tableData.Where(data => FilterCondition(data)).ToList();

            // Apply pagination
            int startIndex = (tablePagination.currentPage - 1) * tablePagination.rowsPerPage;
            return filteredData?.Skip(startIndex).Take(tablePagination.rowsPerPage).ToList();
        }

        // Next page pagination logic
        // private void NextPage()
        // {
        //     // if (currentPage < totalPages)
        //     // {
        //     //     currentPage++;
        //     //     UpdatePagedData();
        //     // }
        // }

        // Previous page pagination logic
        // private void PreviousPage()
        // {
        //     // if (currentPage > 1)
        //     // {
        //     //     currentPage--;
        //     //     UpdatePagedData();
        //     // }
        // }

        // Update the paged data based on current page and page size
        // private void UpdatePagedData()
        // {
        //     // // Ensure the current page is within bounds
        //     // currentPage = Math.Min(currentPage, totalPages);

        //     // pagedData = filteredData.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
        // }

        // private List<MyBlazorWasmApp.Models.TableData> GetPaginatedData()
        // {
        //     int startIndex = (tablePagination.currentPage - 1) * tablePagination.rowsPerPage;
        //     return dashboardData
        //             ?.tableData?.Skip(startIndex)
        //             .Take(tablePagination.rowsPerPage)
        //             .ToList() ?? new List<MyBlazorWasmApp.Models.TableData>();
        // }

        private List<MyBlazorWasmApp.Models.TableData> GetPaginatedData()
        {
            int startIndex = (tablePagination.currentPage - 1) * tablePagination.rowsPerPage;
            var paginatedData =
                dashboardData
                    ?.tableData?.Skip(startIndex)
                    .Take(tablePagination.rowsPerPage)
                    .ToList() ?? new List<MyBlazorWasmApp.Models.TableData>();

            Console.WriteLine(
                $"Current Page: {tablePagination.currentPage}, Data Count: {paginatedData.Count}"
            );
            return paginatedData;
        }

        private void GoToPreviousPage()
        {
            if (tablePagination.currentPage > 1)
            {
                tablePagination.currentPage--;
            }
        }

        private void GoToNextPage()
        {
            if (tablePagination.currentPage < TotalPages)
            {
                tablePagination.currentPage++;
            }
        }

        private void GoToPage(int pageNumber)
        {
            Console.WriteLine(" pageNumber : ", pageNumber);
            tablePagination.currentPage = pageNumber;
        }
    }
}
