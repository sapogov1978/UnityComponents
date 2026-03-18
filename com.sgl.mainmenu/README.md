# com.sgl.mainmenu

Drop-in main menu for Unity mobile games. One component, zero dependencies on your project structure.

## Installation

Package Manager → **+** → Add package from disk → select `package.json`

## Quick Start

```
GameObject → SGL → Main Menu
```

Creates a Canvas, EventSystem, and MainMenu component automatically. Configure in Inspector, hit Play.

## Inspector Reference

### Layout

| Field | Description |
|---|---|
| Banner Height | Top banner height in pixels. Set to 0 to disable. |
| Banner Color | Placeholder color until you integrate an ad SDK. |
| Button Size | Width and height of each icon button in pixels. |
| Button Spacing | Gap between buttons in pixels. |
| Right Padding | Distance from the right screen edge in pixels. |

### Content

| Field | Description |
|---|---|
| Game Title | Displayed on the boot screen and in the menu. |
| Play Label | Text on the Play button. |

### Timing

| Field | Description |
|---|---|
| Fade In Duration | Menu fade-in duration in seconds. |
| Fade Out Duration | Menu fade-out duration in seconds. |
| Boot Duration | How long the boot screen is shown before transitioning to menu. |
| Boot Logo | Optional logo sprite on the boot screen. Falls back to Game Title text. |

### Buttons

Add as many buttons as needed. Each entry exposes:

| Field | Description |
|---|---|
| Label | Name shown in the Inspector for debugging. |
| Icon | Button sprite. |
| Click Sound | AudioClip played on press. |
| Hide On Play | If checked, this button is hidden when gameplay starts. |
| On Click | UnityEvent — wire up any action directly in the Inspector. |

## Integration

### Minimal GameManager wiring

```csharp
[SerializeField] private MainMenu _mainMenu;

void Start()
{
    _mainMenu.OnPlayPressed += OnPlayPressed;
}

private void OnPlayPressed()
{
    _stateMachine.ChangeState(GameState.Playing);
}
```

### Returning to menu after Game Over

```csharp
_mainMenu.ShowMenu();
```

That's it. Boot sequence, fades, button sounds, and safe area handling are all managed internally.

## Events

| Event | When |
|---|---|
| `OnPlayPressed` | Player taps the Play button |
| `OnBootComplete` | Boot screen has finished and menu is visible |