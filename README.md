# Skincare Demo — Unity 2D Casual Game

A casual mobile-style game built in **Unity 2022.3.32f1 (2D)** demonstrating core mechanics commonly found in nail & skincare hyper-casual titles.

---

## Gameplay

Step-by-step nail care routine driven by a centralized flow manager:

| Step | Action | Mechanic |
|------|--------|----------|
| 1 | Spray water | Drag tool + coverage fill |
| 2 | Brush & clean | Dual-layer mask erase |
| 3 | Cut nails | Collider trigger + slide-fade animation |
| 4 | Moisturize | Coverage fill painting |
| 5 | Towel dry | Timed auto-transition |
| 6 | Result screen | Particle FX + UI |

---

## Technical Highlights

**Shader — URP Shader Graph**  
Each paintable layer uses a custom `EraseShader` built in URP Shader Graph. It samples `MainTex` and `MaskTex` (a `Texture2D` written at runtime), multiplies their alpha channels, and outputs to the Fragment node. This drives the visible erase/fill effect entirely on the GPU side.

**`SmoothDelete`**  
CPU-side brush painter that writes alpha values into a `Texture2D` mask using UV mapping from world position. Supports both erase and fill modes with a smooth `SmoothStep` gradient falloff. The mask is uploaded to the shader via `Material.SetTexture("_MaskTex", ...)`.

**`CoverageTracker`**  
Reads pixel alpha values from the mask texture each frame (throttled) and fires a completion callback once a configurable threshold is reached (default 60%).

**`GameFlowManager`**  
Step-based game loop using coroutines. Steps transition automatically via coverage callbacks, timers, or trigger events. `StopAllCoroutines()` is called on each step entry to prevent overlap.

**`DragTool`**  
Reusable drag-and-drop base class. Supports optional snap-back-to-origin and a one-time first-drag event. Extended by `BrushTool`, `SprayerTool`, and `MoisturizeTool`.

**Nail cutting**  
`NailTarget` listens for a `ScissorTip`-tagged collider. On contact, it triggers a cut animation then coroutine-slides the nail down with a fade-out before disabling the object.

---

## Getting Started

1. Clone this repo
2. Open with **Unity 2022.3.32f1**
3. Load the scene in `Assets/Scenes/`
4. Press Play

> Third-party assets are excluded (see `.gitignore`). Missing asset warnings do not affect core logic.

---

*All code and shaders written from scratch as a technical demo.*
