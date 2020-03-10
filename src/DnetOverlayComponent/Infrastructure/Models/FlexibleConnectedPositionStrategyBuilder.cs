using System;
using System.Collections.Generic;
using System.Linq;
using DnetOverlayComponent.Infrastructure.Enums;
using DnetOverlayComponent.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Components;

namespace DnetOverlayComponent.Infrastructure.Models
{
    public struct FlexibleConnectedPositionStrategyBuilder
    {

        private bool _isInitialRender;

        private Tuple<int, int> _lastBoundingBoxSize;

        private bool _isPushed;

        private bool _canPush;

        private bool _growAfterOpen;

        private bool _hasFlexibleDimensions;

        private bool _positionLocked;

        private ClientRect _originRect;

        private ClientRect _overlayRect;

        private ClientRect _viewportRect;

        private int _viewportMargin;

        private List<CdkScrollable> _scrollables;

        private List<ConnectionPositionPair> _preferredPositions;

        private Point _origin;

        // HTMLElement
        private string _pane;

        private bool _isDisposed;

        // HTMLElement
        private string _boundingBox;

        private ConnectedPosition _lastPosition;

        private int? _offsetX;

        private int? _offsetY;

        private string _transformOriginSelector;

        private string _appliedPanelClasses;

        private Point _previousPushAmount;

        private OverlayConfig _overlayConfig;


        public List<ConnectionPositionPair> positions()
        {
            return _preferredPositions;
        }

        public FlexibleConnectedPositionStrategyBuilder AddOverlayConfiguration(OverlayConfig overlayConfig)
        {
            _overlayConfig = overlayConfig;
            return this;
        }


        public FlexibleConnectedPositionStrategyBuilder WithScrollableContainers(List<CdkScrollable> scrollables)
        {
            _scrollables = scrollables;
            return this;
        }

        public FlexibleConnectedPositionStrategyBuilder WithPositions(List<ConnectedPosition> positions)
        {
            var isOnPositions = positions.Contains(_lastPosition);

            if (!isOnPositions) _lastPosition = null;

            ValidatePositions();

            return this;
        }

        public FlexibleConnectedPositionStrategyBuilder WithViewportMargin(int margin)
        {
            _viewportMargin = margin;
            return this;
        }

        public FlexibleConnectedPositionStrategyBuilder WithFlexibleDimensions(bool flexibleDimensions = true)
        {
            _hasFlexibleDimensions = flexibleDimensions;
            return this;
        }

        public FlexibleConnectedPositionStrategyBuilder WithGrowAfterOpen(bool growAfterOpen = true)
        {
            this._growAfterOpen = growAfterOpen;
            return this;
        }

        public FlexibleConnectedPositionStrategyBuilder withPush(bool canPush = true)
        {
            _canPush = canPush;
            return this;
        }

        public FlexibleConnectedPositionStrategyBuilder WithLockedPosition(bool isLocked = true)
        {
            _positionLocked = isLocked;
            return this;
        }

        public FlexibleConnectedPositionStrategyBuilder SetOrigin(Point origin)
        {
            this._origin = origin;
            return this;
        }

        public FlexibleConnectedPositionStrategyBuilder WithDefaultOffsetX(int offset)
        {
            _offsetX = offset;
            return this;
        }

        public FlexibleConnectedPositionStrategyBuilder WithDefaultOffsetY(int offset)
        {
            _offsetY = offset;
            return this;
        }

        public FlexibleConnectedPositionStrategyBuilder WithTransformOriginOn(string selector)
        {
            _transformOriginSelector = selector;
            return this;
        }

        private void ValidatePositions()
        {
            var preferredPositions = this._preferredPositions;

            if (!preferredPositions.Any())
            {
                throw new Exception("FlexibleConnectedPositionStrategy At least one position is required");
            }
        }

        private Point GetOriginPoint(ClientRect originRect, ConnectedPosition pos)
        {

            var x = 0.0;

            if (pos.OriginX == HorizontalConnectionPos.Center)
            {
                // Note: when centering we should always use the `left`
                // offset, otherwise the position will be wrong in RTL.
                x = originRect.Left + (originRect.Width / 2);

            }
            else
            {

                var startX = IsRtl() ? originRect.Right : originRect.Left;

                var endX = IsRtl() ? originRect.Left : originRect.Right;

                x = pos.OriginX == HorizontalConnectionPos.Start ? startX : endX;
            }

            var y = 0.0;

            if (pos.OriginY == VerticalConnectionPos.Center)
            {

                y = originRect.Top + (originRect.Height / 2);

            }
            else
            {

                y = pos.OriginY == VerticalConnectionPos.Top ? originRect.Top : originRect.Bottom;

            }

            return new Point() { X = x, Y = y };
        }

        private Point GetOverlayPoint(Point originPoint, ClientRect overlayRect, ConnectedPosition pos)
        {

            // Calculate the (overlayStartX, overlayStartY), the start of the
            // potential overlay position relative to the origin point.
            var overlayStartX = 0.0;

            if (pos.OverlayX == HorizontalConnectionPos.Center)
            {

                overlayStartX = -overlayRect.Width / 2;

            }
            else if (pos.OverlayX == HorizontalConnectionPos.Start)
            {

                overlayStartX = IsRtl() ? -overlayRect.Width : 0;

            }
            else
            {

                overlayStartX = IsRtl() ? 0 : -overlayRect.Width;
            }

            var overlayStartY = 0.0;

            if (pos.OverlayY == VerticalConnectionPos.Center)
            {

                overlayStartY = -overlayRect.Height / 2;

            }
            else
            {

                overlayStartY = pos.OverlayY == VerticalConnectionPos.Top ? 0 : -overlayRect.Height;
            }

            // The (x, y) coordinates of the overlay.
            return new Point
            {
                X = originPoint.X + overlayStartX,
                Y = originPoint.Y + overlayStartY,
            };
        }

        private OverlayFit GetOverlayFit(Point point, ClientRect overlay, ClientRect viewport, ConnectedPosition position)
        {

            var x = point.X;
            var y = point.Y;

            var offsetX = GetOffset(position, "x");
            var offsetY = GetOffset(position, "y");

            // Account for the offsets since they could push the overlay out of the viewport.
            if (offsetX != null) x += (double)offsetX;

            if (offsetY != null) y += (double)offsetY;

            // How much the overlay would overflow at this position, on each side.
            var leftOverflow = 0 - x;

            var rightOverflow = (x + overlay.Width) - viewport.Width;

            var topOverflow = 0 - y;

            var bottomOverflow = (y + overlay.Height) - viewport.Height;

            // Visible parts of the element on each axis.
            var visibleWidth = this.SubtractOverflows(overlay.Width, new List<double>() { leftOverflow, rightOverflow });

            var visibleHeight = this.SubtractOverflows(overlay.Width, new List<double>() { topOverflow, bottomOverflow });

            var visibleArea = visibleWidth * visibleHeight;

            return new OverlayFit()
            {
                VisibleArea = visibleArea,
                IsCompletelyWithinViewport = (overlay.Width * overlay.Height) == visibleArea,
                FitsInViewportVertically = visibleHeight == overlay.Height,
                FitsInViewportHorizontally = visibleWidth == overlay.Width,
            };
        }

        private bool CanFitWithFlexibleDimensions(OverlayFit fit, Point point, ClientRect viewport)
        {
            if (!this._hasFlexibleDimensions) return false;

            var availableHeight = viewport.Bottom - point.Y;

            var availableWidth = viewport.Right - point.X;

            double.TryParse(GetOverlayConfig().MinHeight, out var minHeight);

            double.TryParse(GetOverlayConfig().MinWidth, out var minWidth);

            var verticalFit = fit.FitsInViewportVertically || (minHeight <= availableHeight);

            var horizontalFit = fit.FitsInViewportHorizontally || (minWidth <= availableWidth);

            return verticalFit && horizontalFit;
        }

        private Point PushOverlayOnScreen(Point start, ClientRect overlay, ViewportScrollPosition scrollPosition)
        {
            // If the position is locked and we've pushed the overlay already, reuse the previous push
            // amount, rather than pushing it again. If we were to continue pushing, the element would
            // remain in the viewport, which goes against the expectations when position locking is enabled.
            if (_previousPushAmount != null && _positionLocked)
            {
                return new Point()
                {
                    X = start.X + this._previousPushAmount.X,
                    Y = start.Y + this._previousPushAmount.Y
                };
            }

            var viewport = this._viewportRect;

            // Determine how much the overlay goes outside the viewport on each
            // side, which we'll use to decide which direction to push it.
            var overflowRight = Math.Max(start.X + overlay.Width - viewport.Right, 0);

            var overflowBottom = Math.Max(start.Y + overlay.Height - viewport.Bottom, 0);

            var overflowTop = Math.Max(viewport.Top - scrollPosition.Top - start.Y, 0);

            var overflowLeft = Math.Max(viewport.Left - scrollPosition.Left - start.X, 0);

            // Amount by which to push the overlay in each axis such that it remains on-screen.
            var pushX = 0.0;
            var pushY = 0.0;

            // If the overlay fits completely within the bounds of the viewport, push it from whichever
            // direction is goes off-screen. Otherwise, push the top-left corner such that its in the
            // viewport and allow for the trailing end of the overlay to go out of bounds.
            if (overlay.Width <= viewport.Width)
            {
                // Estaba esto pushX = overflowLeft || -overflowRight;
                pushX = overflowLeft >= 0 ? overflowLeft : -overflowRight;
            }
            else
            {
                pushX = start.X < this._viewportMargin ? (viewport.Left - scrollPosition.Left) - start.X : 0;
            }

            if (overlay.Height <= viewport.Height)
            {
                // Estaba esto pushY = overflowTop || -overflowBottom;
                pushY = overflowTop >= 0 ? overflowTop : -overflowBottom;
            }
            else
            {
                pushY = start.Y < this._viewportMargin ? (viewport.Top - scrollPosition.Top) - start.Y : 0;
            }

            _previousPushAmount = new Point { X = pushX, Y = pushY };

            return new Point
            {
                X = start.X + pushX,
                Y = start.Y + pushY,
            };
        }

        private void ApplyPosition(ConnectedPosition position, Point originPoint)
        {
            SetTransformOrigin(position);

            //this._setOverlayElementStyles(originPoint, position);

            //this._setBoundingBoxStyles(originPoint, position);

            //if (position.panelClass)
            //{
            //    this._addPanelClasses(position.panelClass);
            //}

            // Save the last connected position in case the position needs to be re-calculated.
            this._lastPosition = position;

            // Notify that the position has been changed along with its change properties.
            // We only emit if we've got any subscriptions, because the scroll visibility
            // calculcations can be somewhat expensive.
            //if (this._positionChanges.observers.length)
            //{
            //    const scrollableViewProperties = this._getScrollVisibility();

            //    const changeEvent = new ConnectedOverlayPositionChange(position, scrollableViewProperties);

            //    this._positionChanges.next(changeEvent);
            //}

            _isInitialRender = false;
        }

        private void SetTransformOrigin(ConnectedPosition position)
        {
            if (!string.IsNullOrEmpty(_transformOriginSelector))
            {
                return;
            }

            //var elements: NodeListOf < HTMLElement > = this._boundingBox!.querySelectorAll(this._transformOriginSelector);

            var xOrigin = "";
            var yOrigin = position.OverlayY;

            if (position.OverlayX.ToString().ToLower() == "center")
            {
                xOrigin = "center";
            }
            else if (IsRtl())
            {
                xOrigin = position.OverlayX.ToString().ToLower() == "start" ? "right" : "left";
            }
            else
            {
                xOrigin = position.OverlayX.ToString().ToLower() == "start" ? "left" : "right";
            }

            //for (var i = 0; i < elements.length; i++)
            //{
            //    elements[i].style.transformOrigin = `${ xOrigin} ${ yOrigin}`;
            //}
        }

        private void SetOverlayElementStyles(Point originPoint, ConnectedPosition position)
        {
            ////var styles = { } as CSSStyleDeclaration;

            //var hasExactPosition = HasExactPosition();

            //var hasFlexibleDimensions = _hasFlexibleDimensions;

            //var config = GetOverlayConfig();

            //if (hasExactPosition)
            //{
            //    var scrollPosition = _viewportRuler.getViewportScrollPosition();

            //    extendStyles(styles, this._getExactOverlayY(position, originPoint, scrollPosition));

            //    extendStyles(styles, this._getExactOverlayX(position, originPoint, scrollPosition));
            //}
            //else
            //{
            //    styles.position = 'static';
            //}

            //// Use a transform to apply the offsets. We do this because the `center` positions rely on
            //// being in the normal flex flow and setting a `top` / `left` at all will completely throw
            //// off the position. We also can't use margins, because they won't have an effect in some
            //// cases where the element doesn't have anything to "push off of". Finally, this works
            //// better both with flexible and non-flexible positioning.
            //let transformString = '';
            //let offsetX = this._getOffset(position, 'x');
            //let offsetY = this._getOffset(position, 'y');

            //if (offsetX)
            //{
            //    transformString += `translateX(${ offsetX}
            //    px) `;
            //}

            //if (offsetY)
            //{
            //    transformString += `translateY(${ offsetY}
            //    px)`;
            //}

            //styles.transform = transformString.trim();

            //// If a maxWidth or maxHeight is specified on the overlay, we remove them. We do this because
            //// we need these values to both be set to "100%" for the automatic flexible sizing to work.
            //// The maxHeight and maxWidth are set on the boundingBox in order to enforce the constraint.
            //// Note that this doesn't apply when we have an exact position, in which case we do want to
            //// apply them because they'll be cleared from the bounding box.
            //if (config.maxHeight)
            //{
            //    if (hasExactPosition)
            //    {
            //        styles.maxHeight = coerceCssPixelValue(config.maxHeight);
            //    }
            //    else if (hasFlexibleDimensions)
            //    {
            //        styles.maxHeight = '';
            //    }
            //}

            //if (config.maxWidth)
            //{
            //    if (hasExactPosition)
            //    {
            //        styles.maxWidth = coerceCssPixelValue(config.maxWidth);
            //    }
            //    else if (hasFlexibleDimensions)
            //    {
            //        styles.maxWidth = '';
            //    }
            //}

            //extendStyles(this._pane.style, styles);
        }

        private bool HasExactPosition()
        {
            return !_hasFlexibleDimensions || _isPushed;
        }

        private bool IsRtl()
        {
            //return _overlayRef.getDirection() === 'rtl';
            return true;
        }

        private double? GetOffset(ConnectedPosition position, string axis)
        {
            if (axis == "x")
            {
                // We don't do something like `position['offset' + axis]` in
                // order to avoid breking minifiers that rename properties.
                return position.OffsetX ?? _offsetX;
            }

            return position.OffsetY ?? this._offsetY;
        }

        /** Subtracts the amount that an element is overflowing on an axis from its length. */
        private double SubtractOverflows(double length, List<double> overflows)
        {

            var amount = overflows.Aggregate(length, (acc, x) => acc - x);

            return amount;
        }

        private OverlayConfig GetOverlayConfig()
        {
            return _overlayConfig;
        }


    }
}
