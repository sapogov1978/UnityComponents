# com.sgl.core.ui

Base UI classes shared across all SGL packages.

## Classes

| Class | Description |
|---|---|
| `UIElement` | Abstract base. Defines `Show()`, `Hide()`, `IsVisible`. |
| `UIFadeable` | Fade via `CanvasGroup`. Inherited by panels and buttons. |
| `UIPanel` | Panel with optional `SetActive(false)` after fade-out. |
| `UIButton` | Button with scale press feedback. Fade comes from the parent panel, not the button itself. |
| `UIFader` | Static coroutine utilities: `FadeIn`, `FadeOut`. |

## Installation

Package Manager → **+** → Add package from disk → select `package.json`
