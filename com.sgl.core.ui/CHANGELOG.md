# Changelog — com.sgl.core.ui

## [1.0.0] - 2026-03-19

### Added
- `UIElement` — abstract base class with `Show()`, `Hide()`, `IsVisible`
- `UIFadeable` — fade in/out via `CanvasGroup`, configurable durations
- `UIPanel` — extends `UIFadeable` with optional `SetActive(false)` on fade-out complete
- `UIButton` — extends `UIFadeable` with scale press feedback; fade delegated to parent panel
- `UIFader` — static `FadeIn` / `FadeOut` coroutine utilities
