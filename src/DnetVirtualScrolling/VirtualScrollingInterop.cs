using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace DnetVirtualScrolling
{
    public static class VirtualScrollingInterop
    {
        public static ValueTask<double> GetElementScrollTop(IJSRuntime jsRuntime, ElementReference element)
        {
            return jsRuntime.InvokeAsync<double>("blginterop.getElementScrollTop", element);
        }

        public static ValueTask<int> GetElementScrollWidth(IJSRuntime jsRuntime, ElementReference element)
        {
            return jsRuntime.InvokeAsync<int>("blginterop.getElementScrollWidth", element);
        }
    }
}
