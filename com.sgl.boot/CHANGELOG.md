# Changelog

All notable changes to `com.sgl.boot` will be documented here.

Format follows [Keep a Changelog](https://keepachangelog.com/en/1.1.0/).  
Version numbers follow [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

---

## [Unreleased]

---

## [1.0.4] — 2026-03-18

### Added

- `BootSpritePostprocessor` — automatically sets Texture Type to Sprite for any texture imported into `com.sgl.boot/Sprites/`. No manual import settings needed.

---

## [1.0.3] — 2026-03-18

### Changed

- `BootCanvasBuilder` now opens a folder picker dialog instead of saving to a hardcoded path. The prefab is created in whichever `Assets/` folder the developer chooses.

---

## [1.0.2] — 2026-03-18

### Changed

- `BootSequenceController` no longer has serialized UI references in the Inspector. Internal references resolved automatically via `transform.Find` in `Awake`. Only `_splashSprite` remains in the Inspector.
- Timings moved to private constants. Not configurable per game.
- Removed `BootScreenConfig` ScriptableObject.
- Removed `BootPanel` adapter.

### Added

- `BootCanvasBuilder` — Editor script that assembles the full `BootCanvas` prefab automatically via `Assets > SGL > Create Boot Prefab`.
- `SGL.Boot.Editor.asmdef` — isolates Editor code from runtime build.
- Runtime logs at each sequence step.
- Null checks in `Awake` with `LogError` per missing reference.
- Guard against calling `StartBoot` twice — `LogWarning` and early return.
- `Validate()` in `BootCanvasBuilder` — checks `SGL_logo.png` is present before building prefab.

---

## [1.0.1] — 2026-03-17

### Removed

- `UnityLogoLayer` and all associated fields from `BootSequenceController`.
- `unity_logo.png` from `Sprites/`.

### Reason

Unity's built-in splash screen already handles the Unity logo before the first scene loads.

---

## [1.0.0] — 2026-03-17

Initial release. Extracted from Tube Runner (Day 1) as a standalone reusable package.

### Added

- `BootSequenceController` — coroutine-based boot sequence.
- `BootScreenConfig` — ScriptableObject for per-game configuration.
- `SGL.Boot.asmdef`.
- `Sprites/` — `SGL_logo.png`.

---

[Unreleased]: https://github.com/sapogov-games/sgl-boot/compare/1.0.4...HEAD
[1.0.4]: https://github.com/sapogov-games/sgl-boot/compare/1.0.3...1.0.4
[1.0.3]: https://github.com/sapogov-games/sgl-boot/compare/1.0.2...1.0.3
[1.0.2]: https://github.com/sapogov-games/sgl-boot/compare/1.0.1...1.0.2
[1.0.1]: https://github.com/sapogov-games/sgl-boot/compare/1.0.0...1.0.1
[1.0.0]: https://github.com/sapogov-games/sgl-boot/releases/tag/1.0.0