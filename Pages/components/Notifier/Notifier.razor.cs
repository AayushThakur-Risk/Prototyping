using Microsoft.AspNetCore.Components;
using MyBlazorWasmApp.Models;

namespace MyBlazorWasmApp.Pages.components.Notifier
{
    public partial class Notifier
    {
        private List<UrlData> Urls = new();
        private string _searchQuery = string.Empty;
        private string _selectedFilter = "All";
        private string _currentSortColumn = "LastScraped";
        private bool IsSortAscending = true;

        private string SearchQuery
        {
            get => _searchQuery;
            set
            {
                _searchQuery = value;
                ApplyFiltersAndSorting();
            }
        }

        private string SelectedFilter
        {
            get => _selectedFilter;
            set
            {
                _selectedFilter = value;
                ApplyFiltersAndSorting();
            }
        }

        private string CurrentSortColumn
        {
            get => _currentSortColumn;
            set
            {
                _currentSortColumn = value;
                IsSortAscending = !IsSortAscending; // Toggle sorting order
                ApplyFiltersAndSorting();
            }
        }

        private List<UrlData> FilteredUrls = new();
        private List<UrlData> PaginatedUrls = new();
        private int ItemsPerPage = 10;
        private int CurrentPage = 1;
        private int TotalPages => (int)Math.Ceiling((double)FilteredUrls.Count / ItemsPerPage);
        private bool CanGoToNextPage => CurrentPage < TotalPages;
        private bool CanGoToPreviousPage => CurrentPage > 1;

        protected override async Task OnInitializedAsync()
        {
            Console.WriteLine("test::::");
            await LoadData();
            ApplyFiltersAndSorting();
        }

        private void TilesClick(char filter)
        {
            var filterMap = new Dictionary<char, string>
            {
                { 'H', "Healthy" },
                { 'A', "All" },
                { 'C', "Changed URLs" },
                { 'B', "Broken" },
                { 'S', "Significant Change" },
            };

            if (filterMap.ContainsKey(filter))
            {
                SelectedFilter = filterMap[filter];
            }
        }

        private void ApplyFiltersAndSorting()
        {
            var filtered = Urls.Where(u =>
                    string.IsNullOrEmpty(SearchQuery)
                    || u.Url.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase)
                )
                .Where(u =>
                    SelectedFilter == "All"
                    || (SelectedFilter == "Healthy" && u.IsHealthy)
                    || (SelectedFilter == "Broken" && !u.IsHealthy)
                )
                .ToList();

            filtered = CurrentSortColumn switch
            {
                "Url" => IsSortAscending
                    ? filtered.OrderBy(u => u.Url).ToList()
                    : filtered.OrderByDescending(u => u.Url).ToList(),
                "SignificanceChange" => IsSortAscending
                    ? filtered.OrderBy(u => u.SignificanceChange).ToList()
                    : filtered.OrderByDescending(u => u.SignificanceChange).ToList(),
                "LastScraped" => IsSortAscending
                    ? filtered.OrderBy(u => u.LastScraped).ToList()
                    : filtered.OrderByDescending(u => u.LastScraped).ToList(),
                _ => filtered,
            };

            FilteredUrls = filtered;
            PaginateUrls();
        }

        private void PaginateUrls()
        {
            PaginatedUrls = FilteredUrls
                .Skip((CurrentPage - 1) * ItemsPerPage)
                .Take(ItemsPerPage)
                .ToList();
        }

        private void GoToNextPage()
        {
            if (CanGoToNextPage)
            {
                CurrentPage++;
                PaginateUrls();
            }
        }

        private void GoToPreviousPage()
        {
            if (CanGoToPreviousPage)
            {
                CurrentPage--;
                PaginateUrls();
            }
        }

        private void ToggleExpand(UrlData urlData)
        {
            urlData.IsExpanded = !urlData.IsExpanded;
        }

        private async Task LoadData()
        {
            await Task.Delay(500); // Simulate loading delay
            var random = new Random();

            // Generate a list of UrlData, each containing a list of SectionData
            Urls = Enumerable
                .Range(1, 20)
                .Select(i => new UrlData
                {
                    Url = $"https://example{i}.com",
                    IsHealthy = random.NextDouble() * 100 < 50,
                    PercentageChange = Math.Round(random.NextDouble() * 100, 1),
                    SignificanceChange = Math.Round(random.NextDouble() * 100, 1),
                    LastScraped = DateTime.Now.AddMinutes(-random.Next(1, 120)),
                    // Add some sample sections
                    Sections = new List<SectionData>
                    {
                        new SectionData
                        {
                            SectionName = "Section 1",
                            PreviousValue = "Old Footer Value",
                            CurrentValue = $"Footer content for example{i}",
                            PercentageChange = Math.Round(random.NextDouble() * 100, 1),
                        },
                        new SectionData
                        {
                            SectionName = "Section 2",
                            PreviousValue = "Old Header Value",
                            CurrentValue = $"Header content for example{i}",
                            PercentageChange = Math.Round(random.NextDouble() * 100, 1),
                        },
                    },
                    IsExpanded = false, // Initially, the sections are not expanded
                })
                .ToList();

            // For debugging: Print out the URLs to verify
            Console.WriteLine("URLs Loaded:");
            foreach (var url in Urls)
            {
                Console.WriteLine($"Url: {url.Url}, Sections Count: {url.Sections.Count}");
            }
        }

        private string NotificationFrequency { get; set; } = "Daily";
    }
}
