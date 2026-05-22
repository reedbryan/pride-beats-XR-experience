# Pride Beats XR Headset Experience

> **XR experience**: Multi‑user AR drumming synced to a 360° video of the Vancouver Pride Parade. This README covers the **headset Unity project** (Quest 3 with passthrough). The PC projection/controller app is documented **[here](https://github.com/reedbryan/cubemap-projection-mediaplayer/tree/master/Assets/PrideBeats)**.

---

## Overview
The headset build is a drumming simulator in AR. Each player wields two virtual drumsticks attached to their tracked hands and hits a single AR drum. Strikes trigger Pride‑inspired visual effects. The headset stays in lock‑step with a PC controller via **OSC** (Open Sound Control):

- The **PC** broadcasts session/control messages.
- The **headset** listens, responds with join/calibration status, and sends **signed** drum‑hit events including an "in‑sync" flag relative to the current note sequence.

Each session begins with a 3‑hit **calibration** so the user learns the interaction before gameplay starts.


## Features
- 🥁 AR drum, interactable via Quest touch sticks (left/right hand)
- 🌈 Drum-hit indicators (notes) syncronized between multiple headsets and supported by pridefull VFX
- 🔗 OSC networking along with a lobby system (ExtOSC) for multi‑headset sessions
- ✅ Signed hit messages back to PC with timing/accuracy fields
- 🕶️ Quest 3's built-in passthrough feature (Mixed Reality)


## Dependencies
- **Unity**: 2022.3 LTS (recommended) — update to match your project
- **ExtOSC** for OSC send/receive
- **Meta/Quest 3** + **Passthrough** enabled (OpenXR/Meta XR)


## Build & install (Quest 3)
1. In Unity, switch to **Android** platform.
2. Enable **XR Plug‑in Management** → **OpenXR** (Meta XR recommended packages).
3. Ensure **Passthrough** feature is on.
4. Build an **APK**.
5. **Sideload** with **SideQuest**:
   - Install SideQuest, connect headset (developer mode must be active).
   - Drag the APK to SideQuest or `adb install -r path/to/PrideBeats.apk`.
   - On the headset: Library → **Unknown Sources** → **PrideBeats**.
6. **Proximity sensor**: add a small piece of tape over the forehead sensor so the app stays running when the headset is off the face (for demos).


## Configuration
Create a `NetworkConfig` asset (ScriptableObject) with:
- **Headset→PC UDP port** (default: `9001`)
- **PC→Headset UDP port** (default: `9000`)
- **PC IP cache** (filled from Session Open broadcast)
- **Signing secret** (string or bytes)
- **Heartbeat interval** (e.g., 1s)

…and a `CalibrationConfig` with:
- **Required hits**: `3`
- **Hit window** / **force threshold** / **timeout**


## Networking & discovery
- Headset listens on **PC→Headset** port for `/SessionOpen`.
- When received, it caches the PC IP carried inside the message.
- Subsequent responses go **Headset→PC** to that IP (unicast). If no cache exists, keep listening for another broadcast.

> The PC computes and sends the network **broadcast** address for `/SessionOpen` (see PC README for details).


## OSC address space (headset)
**Receives**
| Address | Args | Purpose |
|---|---|---|
| `/SessionOpen` | `ip:string`, `port:int` | Store controller IP/port; transition UI to "Session Open" |
| `/Calibrate` | *(none or config)* | Enter 3‑hit calibration mode |
| `/StartGame` | `countdown:float`, `sequence:[…]` | Start countdown and set note sequence for hit‑sync feedback |

**Sends**
| Address | Args | Purpose |
|---|---|---|
| `/Player/Hello` | `id:string` | Optional: announce presence when app starts |
| `/Player/Joined` | `id:string`, `model:string` | Confirm session joined |
| `/Player/Calib` | `id:string`, `done:int(0/1)`, `progress:int` | Report calibration progress (0/3, 1/3, …) |
| `/Hit` | `id:string`, `t:float`, `noteIdx:int`, `inSync:int(0/1)`, `sig:string` | Drum hit with timing and sync flag; **signed** |
| `/Heartbeat` | `id:string`, `t:float` | Liveness ping every N seconds |

> **Signature** (`sig`) is a hex string of a signature computed on a canonical concatenation of the arguments (e.g., `id|t|noteIdx|inSync`) using a shared secret. Keep the algorithm documented in your code (HMAC‑SHA256 or similar). If you already have a scheme, reference it here.


## Calibration flow (headset)
1. On `/Calibrate`, lock UI into calibration mode.
2. Require **3 valid hits** on the AR drum.
3. After each hit, send `/Player/Calib` with `progress` (1/3, 2/3, 3/3).
4. When `progress==3`, send `/Player/Calib` with `done=1` and await `/StartGame`.


## Gameplay timing & sync
- On `/StartGame(countdown, sequence)`, display a countdown and prime the local note sequence.
- When the countdown hits 0, begin evaluating hits against the sequence (±tolerance) to set `inSync`.
- Send `/Hit` on every strike with the evaluated `inSync` and the corresponding `noteIdx` (or `-1` if between notes).


## Developer tips
- Keep physics simple (single collider for the drum head, per‑stick ray/sphere cast or kinematic rigidbodies).
- Add subtle **haptics** on hits (controller vibration) if using Touch controllers.
- Clamp VFX lifetime and pool emitters to avoid per‑frame allocations.
- Add a **failsafe**: if PC is lost, show a reconnect UI and resume listening for `/SessionOpen`.


## Demo media in README
Inline MP4 with controls (commit `docs/demo.mp4`):
```html
<video src="docs/demo.mp4" controls playsinline muted width="720" poster="docs/thumbnail.jpg"></video>
```
Or a small GIF:
```md
![Headset Demo](docs/demo.gif)
```


## Troubleshooting
- **No session appears**: ensure PC is broadcasting on the correct subnet; headset and PC must be on the same Wi‑Fi.
- **Firewall**: allow UDP on the configured ports.
- **Passthrough black**: confirm Meta XR Passthrough is enabled and granted.
- **App pauses off‑face**: add tape over proximity sensor or set appropriate XR flags.


## Credits & licenses
- 360° video footage: Vancouver Pride Parade (credit details here)
- Networking: ExtOSC
- Icons/graphics/VFX: (list your sources)
