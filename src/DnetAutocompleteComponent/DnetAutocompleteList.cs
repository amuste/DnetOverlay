using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using DnetAutocompleteComponent.Infrastructure.Services;
using DnetOverlayComponent.Infrastructure.Enums;
using DnetOverlayComponent.Infrastructure.Interfaces;
using DnetOverlayComponent.Infrastructure.Models;
using DnetOverlayComponent.Infrastructure.Services;
using DnetOverlayComponent.Infrastructure.Services.CssBuilder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace DnetAutocompleteComponent
{
    public class DnetAutoCompleteList<TItem> : InputBase<string>
    {
        [Inject]
        private IOverlayService OverlayService { get; set; }

        [Inject]
        private DnetOverlayInterop DnetOverlayInterop { get; set; }


        [Parameter]
        public Func<TItem, string> DisplayValueConverter { get; set; }

        [Parameter]
        public Func<TItem, string> SearchValueConverter { get; set; }

        [Parameter]
        public Type CellTemplate { get; set; } = null;

        [Parameter]
        public List<TItem> Items { get; set; } = new List<TItem>();

        [Parameter]
        public string Width { get; set; } = null;

        [Parameter]
        public string Height { get; set; } = null;

        [Parameter]
        public string MinWidth { get; set; } = null;

        [Parameter]
        public string MinHeight { get; set; } = null;

        [Parameter]
        public string MaxWidth { get; set; } = null;

        [Parameter]
        public string MaxHeight { get; set; } = null;

        [Parameter]
        public int DebounceTime { get; set; } = 250;

        [Parameter]
        public bool InputTextToUpper { get; set; } = false;

        [Parameter]
        public EventCallback<TItem> OnItemSelected { get; set; }

        [Parameter]
        public bool HasContent { get; set; } = false;


        private ElementReference _menuTrigger;

        private bool _isOpen;

        private bool _isClosing;

        private OverlayReference _menuReference;

        private Timer _debounceTimer;

        private string _autocompleteContainerStyle = "";

        private string _inputContentWrapperCssClasses = "";

        private RenderFragment _inputContent { get; set; }

        private AutoCompleteListService<TItem> _autoCompleteListService;


        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            var s = -1;
            if (HasContent) builder.OpenElement(s++, "div");
            if (HasContent) builder.AddAttribute(s++, "style", _autocompleteContainerStyle);
            builder.OpenElement(s++, "input");
            builder.AddMultipleAttributes(s++, AdditionalAttributes);
            builder.AddAttribute(s++, "class", GetCssClasses());
            builder.AddAttribute(s++, "value", BindConverter.FormatValue(CurrentValue));
            builder.AddAttribute(s++, "autocomplete", "off");
            builder.AddAttribute(s++, "oninput", EventCallback.Factory.CreateBinder<string>(this, __value => CurrentValueAsString = __value, CurrentValueAsString));
            builder.AddAttribute(s++, "onfocusin", EventCallback.Factory.Create<MouseEventArgs>(this, OpenMenu));
            builder.AddAttribute(s++, "onkeyup", EventCallback.Factory.Create<KeyboardEventArgs>(this, HandleKeyUp));
            builder.AddAttribute(s++, "onkeydown", EventCallback.Factory.Create<KeyboardEventArgs>(this, HandleKeydown));
            builder.AddAttribute(s++, "onblur", EventCallback.Factory.Create<FocusEventArgs>(this, ClosenMenu));
            builder.AddEventStopPropagationAttribute(s++, "onkeyup", true);
            builder.AddEventPreventDefaultAttribute(s++, "onkeyup", true);
            builder.SetUpdatesAttributeName("value");
            builder.AddElementReferenceCapture(s++, value => _menuTrigger = value);
            builder.CloseElement();
            if (!HasContent) return;
            builder.OpenElement(s++, "div");
            builder.AddAttribute(s++, "class", _inputContentWrapperCssClasses);
            __Blazor.DnetAutocompleteComponent.DnetAutoCompleteList.TypeInference.CreateCascadingValue_0(builder, s++, s++, this, s++,
                (builder1) =>
                {
                    builder1.AddContent(s++, _inputContent);
                });
            builder.CloseElement();
            builder.CloseElement();
        }

        protected override void OnInitialized()
        {
            _debounceTimer = new Timer(DebounceTime);

            _debounceTimer.Elapsed += OnUserFinish;

            _debounceTimer.AutoReset = false;

            if (HasContent) GetInputContentWrapperCssClasses();

            _autocompleteContainerStyle = new StyleBuilder("position", "relative")
                .AddStyle("width", "100%")
                .Build();

            if (DisplayValueConverter == null) DisplayValueConverter = SearchValueConverter;
        }

        protected override void OnAfterRender(bool firstRender)
        {
            if (!firstRender) return;

            _autoCompleteListService = new AutoCompleteListService<TItem>();

            _autoCompleteListService.OnItemSelected += ItemSelected;
        }

        private RenderFragment CreateInputContent(TItem item)
        {
            if (!typeof(ComponentBase).IsAssignableFrom(CellTemplate) && CellTemplate != null)
            {
                throw new ArgumentException($"{CellTemplate.FullName} must be a Blazor Component");
            }

            var inputContent = new RenderFragment(x =>
            {
                var s = -1;
                x.OpenComponent(s++, CellTemplate);
                x.AddAttribute(s++, "Item", item);
                x.CloseComponent();
            });

            return inputContent;
        }

        protected override bool TryParseValueFromString(string value, out string result, out string validationErrorMessage)
        {
            result = InputTextToUpper ? value.ToUpper() : value;
            validationErrorMessage = null;
            return true;
        }

        private string GetCssClasses()
        {
            var classes = new CssBuilder()
                .AddClass("dnet-autoc-input-autocomplete")
                .AddClass("padding-left", when: HasContent)
                .AddClass(CssClass, when: !string.IsNullOrEmpty(CssClass))
                .Build();

            return classes;
        }

        private void GetInputContentWrapperCssClasses()
        {
            _inputContentWrapperCssClasses = new CssBuilder()
                .AddClass("dnet-autoc-input-content-wrapper")
                .AddClass("hide", when: string.IsNullOrEmpty(CurrentValue))
                .Build();
        }

        private void HandleKeydown(KeyboardEventArgs data)
        {
            if (data.Key != "Tab") return;
            _debounceTimer.Stop();
            _isClosing = true;
        }

        private void HandleKeyUp(KeyboardEventArgs data)
        {
            if (data.Key == "Tab")
            {
               return;
            }

            _debounceTimer.Stop();

            _debounceTimer.Start();
        }

        private async void OnUserFinish(Object source, ElapsedEventArgs e)
        {
            await InvokeAsync(() =>
            {
                var items = Items.Where(p => SearchValueConverter(p).ToUpper().Contains(CurrentValue.ToUpper())).ToList();
                _autoCompleteListService.UdateList(items);

                if (!HasContent) return;
                GetInputContentWrapperCssClasses();
                StateHasChanged();
            });
        }

        private void OpenMenu()
        {
            if (_isOpen) return;

            DnetOverlayInterop.Focus(_menuTrigger);

            AttachMenu();

            _isOpen = true;
        }

        private void ClosenMenu(FocusEventArgs data)
        {
            if (_isOpen && _isClosing)
            {
                _isClosing = false;
                DetachMenu();
            }
        }

        private async void ItemSelected(TItem item)
        {
            var value = SearchValueConverter(item);

            Value = value;
            await ValueChanged.InvokeAsync(value);

            EditContext?.NotifyFieldChanged(FieldIdentifier);

            await OnItemSelected.InvokeAsync(item);

            if (HasContent)
            {
                _inputContent = CreateInputContent(item);
                GetInputContentWrapperCssClasses();
            }

            DetachMenu();

            StateHasChanged();
        }

        private void AttachMenu()
        {
            var positions = new List<ConnectedPosition>
            {
                new ConnectedPosition
                {
                    OriginX = HorizontalConnectionPos.Start,
                    OriginY = VerticalConnectionPos.Bottom,
                    OverlayX = HorizontalConnectionPos.Start,
                    OverlayY = VerticalConnectionPos.Top
                },
                new ConnectedPosition
                {
                    OriginX = HorizontalConnectionPos.Start,
                    OriginY = VerticalConnectionPos.Top,
                    OverlayX = HorizontalConnectionPos.Start,
                    OverlayY = VerticalConnectionPos.Bottom
                },
                new ConnectedPosition
                {
                    OriginX = HorizontalConnectionPos.End,
                    OriginY = VerticalConnectionPos.Bottom,
                    OverlayX = HorizontalConnectionPos.End,
                    OverlayY = VerticalConnectionPos.Top
                },
                new ConnectedPosition
                {
                    OriginX = HorizontalConnectionPos.End,
                    OriginY = VerticalConnectionPos.Top,
                    OverlayX = HorizontalConnectionPos.End,
                    OverlayY = VerticalConnectionPos.Bottom
                }
            };

            var flexibleConnectedPositionStrategyBuilder = new FlexibleConnectedPositionStrategyBuilder()
                .WithViewportMargin(8)
                .SetOrigin(_menuTrigger)
                .WithFlexibleDimensions(false)
                .WithPositions(positions);

            var connectedPanelConfig = new OverlayConfig
            {
                HasBackdrop = true,
                HasTransparentBackdrop = true,
                PositionStrategy = PositionStrategy.FlexibleConnectedTo,
                FlexibleConnectedPositionStrategyBuilder = flexibleConnectedPositionStrategyBuilder,
                MinWidth = MinWidth,
                MaxWidth = MaxWidth,
                MinHeight = MinHeight,
                MaxHeight = MaxHeight,
                BackdropZindex = 1001,
                PanelZindex = 1100
            };

            var items = new List<TItem>();

            if (!string.IsNullOrEmpty(CurrentValue))
            {
                items = Items.Where(p => SearchValueConverter(p).ToUpper().Contains(CurrentValue.ToUpper())).ToList();
                _autoCompleteListService.UdateList(items);
            }
            else
            {
                items = Items;
            }

            var s = -1;
            var menuContent = new RenderFragment(x =>
            {
                x.OpenComponent(s++, typeof(DnetAutocompleteListPanel<TItem>));
                x.AddAttribute(s++, "Items", items);
                x.AddAttribute(s++, "Width", Width ?? "100%");
                x.AddAttribute(s++, "Height", Height ?? "200px");
                x.AddAttribute(s++, "MinWidth", MinWidth);
                x.AddAttribute(s++, "MaxWidth", MaxWidth);
                x.AddAttribute(s++, "MinHeight", MinHeight);
                x.AddAttribute(s++, "MaxHeight", MaxHeight);
                x.AddAttribute(s++, "AutoCompleteListService", _autoCompleteListService);
                x.AddAttribute(s++, "DisplayValueConverter", DisplayValueConverter);
                x.AddAttribute(s++, "CellTemplate", CellTemplate);
                x.CloseComponent();
            });

            _menuReference = OverlayService.Attach(menuContent, connectedPanelConfig);

            _menuReference.Close += CloseFilter;
        }

        private void DetachMenu()
        {
            _isOpen = false;

            var result = new OverlayResult
            {
                OverlayRef = _menuReference.GetOverlayReferenceId(),
                CloseReason = CloseReason.Cancel
            };

            OverlayService.Detach(result);
        }

        void CloseFilter(OverlayResult overlayDataResult)
        {
            _isOpen = false;
        }

        public void Dispose()
        {
            if (_menuReference != null) _menuReference.Close -= CloseFilter;

            _autoCompleteListService.OnItemSelected -= ItemSelected;
        }
    }
}

namespace __Blazor.DnetAutocompleteComponent.DnetAutoCompleteList
{
    internal static class TypeInference
    {
        public static void CreateCascadingValue_0<TValue>(RenderTreeBuilder builder, int seq, int seq0, TValue arg0, int seq1, RenderFragment arg1)
        {
            builder.OpenComponent<CascadingValue<TValue>>(seq);
            builder.AddAttribute(seq0, "Value", arg0);
            builder.AddAttribute(seq1, "ChildContent", arg1);
            builder.CloseComponent();
        }
    }
}
