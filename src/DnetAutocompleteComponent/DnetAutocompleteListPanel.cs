using System;
using System.Collections.Generic;
using System.Linq;
using DnetAutocompleteComponent.Infrastructure.Services;
using DnetOverlayComponent.Infrastructure.Interfaces;
using DnetOverlayComponent.Infrastructure.Services.CssBuilder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace DnetAutocompleteComponent
{
    public class DnetAutocompleteListPanel<TItem> : ComponentBase, IDisposable
    {
        [Inject]
        private IOverlayService OverlayService { get; set; }

        [Inject]
        private IJSRuntime JSRuntime { get; set; }

        [CascadingParameter]
        private int OverlayReferenceId { get; set; }

        [Parameter]
        public List<TItem> Items { get; set; }

        [Parameter]
        public string Width { get; set; } = "200px";

        [Parameter]
        public string Height { get; set; } = "200px";

        [Parameter]
        public string MinWidth { get; set; } = null;

        [Parameter]
        public string MinHeight { get; set; } = null;

        [Parameter]
        public string MaxWidth { get; set; } = null;

        [Parameter]
        public string MaxHeight { get; set; } = null;

        [Parameter]
        public AutoCompleteListService<TItem> AutoCompleteListService { get; set; }

        [Parameter]
        public Func<TItem, string> DisplayValueConverter { get; set; }

        [Parameter]
        public Type CellTemplate { get; set; } = null;

        [Parameter]
        public int ItemHeight { get; set; } = 40;

        [Parameter]
        public int ContainerHeight { get; set; } = 200;


        private string _styles { get; set; } = null;

        private ElementReference _eVirtualScrollVieport { get; set; }

        private string _totalContentHeight { get; set; }

        private string _transformY { get; set; } = "0px";

        private List<TItem> _items { get; set; } = new List<TItem>();


        protected override void BuildRenderTree(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder builder)
        {
            var seq = -1;
            builder.OpenElement(seq++, "div");
            builder.AddAttribute(seq++, "class", "dnet-autoc-menu-container dnet-virtual-scroll-viewport dnet-virtual-scroll-orientation-vertical");
            builder.AddAttribute(seq++, "style", _styles);
            builder.AddAttribute(seq++, "onscroll", EventCallback.Factory.Create<EventArgs>(this, _ => OnScroll()));
            builder.AddElementReferenceCapture(seq++, value => _eVirtualScrollVieport = value);
            builder.OpenElement(seq++, "div");
            builder.AddAttribute(seq++, "class", "dnet-autoc-menu-container-filter-wrapper dnet-virtual-scroll-content-wrapper");
            builder.AddAttribute(9, "style", "transform:" + " translateY(" + (_transformY) + ")");
            foreach (var item in _items)
            {
                builder.AddContent(seq++, "");
                builder.OpenElement(seq++, "div");
                builder.AddAttribute(seq++, "class", "dnet-autoc-menu-item");
                builder.AddAttribute(seq++, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, _ => SelectItem(item)));
                builder.AddAttribute(seq++, "style", $"margin-bottom: 2px; height:{ItemHeight}");
                if (CellTemplate != null && typeof(ComponentBase).IsAssignableFrom(CellTemplate))
                {
                    builder.AddContent(seq++, "");
                    builder.OpenElement(seq++, "div");
                    builder.AddAttribute(seq++, "class", "dnet-autoc-content-wrapper");
                    builder.AddContent(seq++, CreateCellTemplateComponent(item, CellTemplate));
                    builder.OpenElement(seq++, "span");
                    builder.AddAttribute(seq++, "class", "dnet-autoc-content-text has-content");
                    builder.AddContent(seq++, DisplayValueConverter(item));
                    builder.CloseElement();
                    builder.CloseElement();
                }
                else
                {
                    builder.AddContent(seq++, "");
                    builder.OpenElement(seq++, "span");
                    builder.AddAttribute(seq++, "class", "dnet-autoc-content-text");
                    builder.AddContent(seq++, DisplayValueConverter(item));
                    builder.CloseElement();
                }
                builder.AddContent(seq++, "");
                builder.CloseElement();
            }
            builder.AddContent(seq++, "");
            builder.CloseElement();
            builder.OpenElement(22, "div");
            builder.AddAttribute(23, "class", "dnet-virtual-scroll-spacer");
            builder.AddAttribute(24, "style", "height:" + " " + (_totalContentHeight) + ";");
            builder.CloseElement();
            builder.CloseElement();
        }

        protected override void OnInitialized()
        {
            _styles = GetStyles();

            _totalContentHeight = $"{Items.Count * ItemHeight}px";

            var takeItems = ContainerHeight / ItemHeight;

            _items = Items.Skip(0).Take(takeItems).Select(p => p).ToList();

            AutoCompleteListService.OnUpdateList += ListUpdated;
        }

        private string GetStyles()
        {
            var styles = new StyleBuilder()
                .AddStyle("width", Width, when: !string.IsNullOrEmpty(Width))
                .AddStyle("height", Height, when: !string.IsNullOrEmpty(Height))
                .AddStyle("min-height", MinHeight, when: !string.IsNullOrEmpty(MinHeight))
                .AddStyle("min-width", MinWidth, when: !string.IsNullOrEmpty(MinWidth))
                .AddStyle("max-width", MaxWidth, when: !string.IsNullOrEmpty(MaxWidth))
                .AddStyle("max-height", MaxHeight, when: !string.IsNullOrEmpty(MaxHeight))
                .AddStyle("transform", $"transform: translateY(@_transformY)")
                .Build();

            return styles;
        }

        private RenderFragment CreateCellTemplateComponent(TItem item, Type componeType) => builder =>
        {
            builder.OpenComponent(0, componeType);
            builder.AddAttribute(1, "Item", item);
            builder.CloseComponent();
        };

        private void ListUpdated(List<TItem> items)
        {
            Items = items;

            _totalContentHeight = $"{Items.Count * ItemHeight}px";

            var takeItems = ContainerHeight / ItemHeight;

            _items = Items.Skip(0).Take(takeItems).Select(p => p).ToList();

            StateHasChanged();
        }

        private void SelectItem(TItem item)
        {
            AutoCompleteListService.UpdateSelectedItem(item);
        }

        private async void OnScroll()
        {
            var scrollTop = await VirtualScrollingInterop.GetElementScrollTop(JSRuntime, _eVirtualScrollVieport);

            var skipItems = (int)Math.Ceiling(scrollTop) / ItemHeight;

            var takeItems = ContainerHeight / ItemHeight;

            _items = Items.Skip(skipItems).Take(takeItems).Select(p => p).ToList();

            var offset = ItemHeight * skipItems;

            _transformY = $"{offset}px";

            StateHasChanged();
        }

        public void Dispose()
        {
            AutoCompleteListService.OnUpdateList -= ListUpdated;
        }
    }
}
