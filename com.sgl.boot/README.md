# com.sgl.boot

Reusable boot sequence for all Sapogov Games Lab Unity projects.

---

## Sequence

```
[Unity built-in splash]   — Player Settings > Splash Screen, outside our control
        ↓
   SGL logo               fade from black → hold → fade to black
        ↓
  Splash screen           fade from black → hold → dissolve into main menu
```

The main menu renders behind the boot canvas throughout. The splash dissolves to transparent, revealing the menu underneath — no cut, no fade to black.

---

## Requirements

- Unity 2022.3 LTS or newer
- UnityEngine.UI (built-in)
- No third-party dependencies

---

## Installation

Add to `Packages/manifest.json`:

**Local path:**
```json
{
  "dependencies": {
    "com.sgl.boot": "file:../../sgl-packages/com.sgl.boot"
  }
}
```

**Git URL:**
```json
{
  "dependencies": {
    "com.sgl.boot": "https://github.com/sapogov1978/UnityComponents.git?path=com.sgl.boot"
  }
}
```

To pin a specific version, append a tag: `...UnityComponents.git?path=com.sgl.boot#1.0.4`

---

## Usage

**1. Create the prefab** (once per project):

`Assets > SGL > Create Boot Prefab`

A folder picker opens — choose any folder inside `Assets/`. The prefab is saved there with the full hierarchy assembled and all internal references wired.

**2. Drop the prefab into the scene.**

**3. Assign Splash Sprite** on `BootSequenceController` in the Inspector.

**4. Call from GameManager:**

```csharp
[SerializeField] private SGL.Boot.BootSequenceController _bootSequence;

void Start()
{
    _bootSequence.StartBoot(OnBootComplete);
}
```

---

## Adding SGL_logo.png

Drop `SGL_logo.png` into `com.sgl.boot/Sprites/`. The package includes an Asset Postprocessor that automatically sets Texture Type to Sprite on import — no manual steps needed.

If the file was added before the postprocessor was in place, right-click it in the Project window → **Reimport**.

---

## Timings

Hardcoded constants in `BootSequenceController`. Not intended to be changed per game.

| Stage | Fade in | Hold | Fade out / Dissolve |
|---|---|---|---|
| SGL logo | 0.6 s | 1.8 s | 0.6 s |
| Splash | 0.8 s | 1.5 s | 1.2 s |

---

## File structure

```
com.sgl.boot/
├─ package.json
├─ README.md
├─ CHANGELOG.md
├─ Runtime/
│  ├─ BootSequenceController.cs
│  └─ SGL.Boot.asmdef
├─ Editor/
│  ├─ BootCanvasBuilder.cs
│  ├─ BootSpritePostprocessor.cs
│  └─ SGL.Boot.Editor.asmdef
└─ Sprites/
   └─ SGL_logo.png
```

---

## License

Internal SGL tooling. Not for redistribution.