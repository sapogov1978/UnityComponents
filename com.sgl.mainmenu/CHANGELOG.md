# Changelog

All notable changes to `com.sgl.mainmenu` will be documented here.

Format follows [Keep a Changelog](https://keepachangelog.com/en/1.1.0/).  
Version numbers follow [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

---

## [1.0.0] - 2026-03-19

### Added
- `MainMenu` component — single MonoBehaviour, drag onto Canvas and configure in Inspector
- Boot screen with configurable duration and optional logo sprite; falls back to game title text
- Top banner zone with safe area handling and configurable placeholder color
- Right-side buttons panel built from a serialized `MenuButtonData` array
- Per-button configuration: icon sprite, click sound, hide-on-play flag, UnityEvent
- Menu content panel: game title, best score (reads `PlayerPrefs`), Play button
- Fade in/out for menu and buttons panels with configurable durations
- `OnPlayPressed` and `OnBootComplete` C# events for GameManager integration
- `ShowMenu()` / `ShowGameplay()` public API for state transitions
- `GameObject → SGL → Main Menu` editor shortcut — creates Canvas, EventSystem, and component in one click
- Custom Inspector with canvas validation warning
- Automatic `AudioSource` creation for button click sounds
- Unity 2022.3 LTS support