***Documentation under development***

DnetOverlay v1.0.0

The `DnetOverlay` package provides a way to open floating panels on the screen.

### Creating overlays
Calling `OverlayReference Attach(RenderFragment overlayContent, OverlayConfig overlayConfig)` will return an `OverlayReference` instance. This instance is a handle for managing that specific overlay.

### Configuring an overlay
When creating an overlay, an optional configuration object can be provided.
```ts
 var globalPositionStrategy = new GlobalPositionStrategyBuilder();

 globalPositionStrategy.CenterVertically(null);
 globalPositionStrategy.CenterHorizontally(null);

 var overlayConfig = new OverlayConfig()
 {
    HasBackdrop = true,
    HasTransparentBackdrop = false,
    Width = "600px",
    Height = "400px",
    GlobalPositionStrategy = globalPositionStrategy
 };
```

#### Position strategies
The `positionStrategy` configuration option determines how the overlay will be positioned on-screen.
There are two position strategies available as part of the library: `GlobalPositionStrategy` and
`ConnectedPositionStrategy/ Not developed yet`.

`GlobalPositionStrategy` is used for overlays that require a specific position in the viewport,
unrelated to other elements. This is commonly used for modal dialogs and application-level
notifications.

### The overlay container
The `OverlayContainer` provides a handle to the container element in which all individual overlay
elements are rendered. 

