using Microsoft.AspNetCore.Components;

namespace MyBlazorWasmApp.Pages
{
    public partial class Count
    {
        private int clickCount = 0;

        private void IncrementCount()
        {
            clickCount++;
        }
    }
}
