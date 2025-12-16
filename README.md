# Vo Lam Tu Tien – MVP

**Vo Lam Tu Tien** is a 3D ARPG inspired by classic martial-arts MMORPGs, but set in a **cultivation (Xianxia) world**: you start as a low-level cultivator, train internal arts, break through realms, and hunt bosses to obtain stronger manuals and artifacts.

This document introduces the **MVP (Minimum Viable Product)** – a prototype focused on the core loop, auto-farming feel, and the Internal Art / Realm system.

---

## 1. Overview

- **Genre:** Top-down 3D ARPG, auto-farm, cultivation (Xianxia)  
- **Engine:** Unity (3D)  
- **Platform:** PC (Windows)  
- **Mode:** Singleplayer (offline for MVP)

### MVP Goals

The MVP focuses on:

- Basic movement & combat (right-click to move, auto-farm normal mobs).
- **Internal Art & Realm** system (replacing traditional character levels).
- 2 main Internal Arts:
  - `So Tam Cong Quyet` – starter internal art, focused on **regeneration & stable auto-farm**.
  - `Thanh Van Kiem Quyet` – sword-cultivator internal art, focused on **damage & “Kiem Hon” stacks**.
- 1 training field + 1 dungeon with a **Qi Refinement boss**:
  - Training field: farm Internal Art EXP and basic gear.
  - Boss: drops **advanced Internal Art** (`Thanh Van Kiem Quyet`).
- Core feeling:
  > auto-farm → cultivate internal art → break through realm → hunt boss → unlock stronger internal art.

---

## 2. Core Gameplay Loop (MVP)

Typical 5–20 minute loop in MVP:

1. **Start in the Beginner Village**
   - Player starts with:
     - `So Tam Cong Quyet` – Internal Art Tier 1 (Qi Refinement 1).
   - Tutorial:
     - Move with right-click.
     - Basic attacks vs simple mobs.

2. **Go to the Qi Refinement Training Field**
   - Right-click to move to the mob area.
   - Turn on **auto-farm**:
     - The character will automatically seek and attack nearby normal mobs.

3. **Farm mobs → gain Internal Art EXP (KNNC)**
   - Each mob kill:
     - Grants **Internal Art EXP (KNNC)** to the active Internal Art.
     - Drops gold, minor spirit stones, basic items.
   - When KNNC reaches thresholds:
     - Internal Art **Lv** increases → reaching breakpoints → **Tier** increases → **Realm** increases.
   - With `So Tam Cong Quyet`: goes from **Qi Refinement 1 → 9**.

4. **Cultivate to Qi Refinement 9**
   - `So Tam Cong Quyet` has **9 Tiers**, mapped to Qi Refinement 1–9.
   - Each Tier:
     - Increases basic stats (HP/ATK/MP as per data).
     - Enhances the passive skill `Tinh Nguyen` (HP/MP regen every 5 seconds).
   - At Tier 9:
     - Player hits the **“soft cap”** for the starter art and needs **a stronger manual** to progress further.

5. **Unlock the Qi Refinement Secret Realm – Boss Fight**
   - Upon reaching Qi Refinement 9:
     - Player can enter the **Qi Refinement Secret Realm** with boss `Ho Yeu Luyen Khi`.
   - Killing the boss:
     - Grants a large amount of KNNC, gold, green items.
     - Most importantly: drops **`Thanh Van Kiem Quyet`** (guaranteed on the first kill).

6. **Switch to Thanh Van Kiem Quyet – New cultivation cycle**
   - Player chooses to **train `Thanh Van Kiem Quyet`** as the active Internal Art:
     - Each Tier grants significantly better stats.
     - High tiers (8–9) can be treated as **Foundation Establishment 1–2** for future content.
     - Unlocks the unique stacking skill `Kiem Hon`, greatly boosting boss DPS.

---

## 3. Internal Arts & Realms

### 3.1. Replacing character level with Internal Art

- The game does **not** use traditional character XP/level.
- Instead:
  - Killing mobs → grants **Internal Art EXP (KNNC)**.
  - KNNC → increases **Internal Art Lv** → at breakpoints → **Tier** increases → **Realm** increases.

### 3.2. Tiers & Realms (MVP)

- Each Internal Art in MVP has **9 Tiers**.
- Simple mapping:
  - Tier 1–9 = **Qi Refinement 1–9**.
- For advanced arts like `Thanh Van Kiem Quyet`, high tiers (8–9) can be considered **Foundation Establishment 1–2** when checking requirements for future maps/gear.

---

## 4. Internal Arts in MVP

### 4.1. So Tam Cong Quyet

- **Type:** Basic Internal Art (Common).  
- **Role:** Early-game comfort pick — **easy to train, strong sustain, safe auto-farming**.  
- **Tiers:** 1–9, mapped to Qi Refinement 1–9.

#### Internal Art Skill: Tinh Nguyen (Passive – Auto regen)

- Behavior:
  - Every **5 seconds**, automatically restore a **fixed amount of HP and MP**, based on the current Tier.
  - Works regardless of:
    - Being in combat or idle.
    - Attacking or not.
  - Clamped to MaxHP/MaxMP (no overheal).

- Example tick values (for prototyping):

| Tier | HP per 5s | MP per 5s |
|------|-----------|-----------|
| 1    | 6         | 3         |
| 3    | 12        | 7         |
| 5    | 20        | 11        |
| 9    | 45        | 22        |

> Full detailed table (1–9) is available in the design/Excel files.

#### Tier 9 Upgrade: Hoi Nguyen Tran

When `So Tam Cong Quyet` reaches Tier 9:

- `Tinh Nguyen` is upgraded:
  - HP/MP per-tick (every ~5s) increased by about **20%**.
- Adds a **periodic burst effect** (fully automatic):
  - Every ~90 seconds:
    - Instantly restore about **12% MaxHP** + **15% MaxMP**.
    - Then, for 3 seconds:
      - +4% MaxHP per second.
      - +3% MaxMP per second.
- Overall: at Tier 9, the character has **very strong passive sustain for long auto-farming sessions**.

---

### 4.2. Thanh Van Kiem Quyet

- **Type:** Rare Internal Art (Sword cultivator).  
- **Source:** Drops from the Qi Refinement Secret Realm Boss.  
- **Role:** Provides a big **DPS boost**, especially against bosses/elites.

#### Internal Art Skill: Kiem Hon (Passive – Stacking on target)

- Base behavior:
  - Each time the player’s attack/skill hits an enemy:
    - Apply **1 stack of Kiem Hon** (up to 3 stacks per target).
    - Each stack lasts about 6 seconds (refreshed on re-application).
  - Each stack:
    - Increases the **damage that target takes from the player** by a percentage.
  - When a target reaches 3 stacks:
    - The stacks **automatically explode**:
      - Deal bonus damage based on the player’s ATK (explosion).
      - Deal small AOE damage around the target.

- All stack bonuses and explosion multipliers **scale with Tier**, resulting in:
  - Noticeably higher burst damage vs. bosses at high Tiers.

#### Tier 9 Upgrade: Thien Van Kiem Hon

When `Thanh Van Kiem Quyet` reaches Tier 9:

- Kiem Hon gets upgraded:
  - Higher **per-stack damage amplification** (for example, ~10% per stack at Tier 9).
  - Explosion:
    - Main damage multiplier ×1.4.
    - Adds a **true damage component** (ignores defense) based on the explosion damage.
    - Wider and stronger AOE.
- Optional add-ons (for later versions, if needed):
  - Chance to spawn a chaining “Kiem Van” projectile:
    - Flies to another nearby enemy, dealing a portion of the explosion damage and applying a stack.
  - Short buff on the player (6s) after each explosion:
    - Increased Crit Chance and Attack Speed.

> In the initial MVP build, you can start with a simpler version (stacks → explosion → higher damage) and incrementally add chain & buff later.

---

## 5. Auto-Farm (MVP)

- Auto toggle (e.g. key `T` or a UI button).
- When **Auto ON**:
  - The character:
    - Automatically searches for the nearest normal mob.
    - Moves towards it (reusing the click-to-move logic).
    - Attacks until it dies.
    - Repeats.

Synergy with Internal Arts:

- `So Tam Cong Quyet`:
  - `Tinh Nguyen` maintains HP/MP → **stable long-term auto-farm**.
- `Thanh Van Kiem Quyet`:
  - `Kiem Hon` stacks and explodes → **high DPS** on enemies that survive multiple hits (bosses, elites).

---

## 6. Basic Controls (MVP)

- **Right Mouse Button:**
  - Click on ground → move to clicked position.
- **Attacks:**
  - MVP: can be auto-attacks whenever in range (especially in auto-farm mode), or via click-on-enemy (to be tuned).
- **Auto-farm:**
  - Default key: `T` (subject to change).

---

## 7. Current Status & Roadmap

As of the MVP design:

- Covered / designed:
  - Internal Arts & Realm system (So Tam Cong Quyet, Thanh Van Kiem Quyet).
  - Internal Art skills: `Tinh Nguyen` (regen), `Kiem Hon` (stacks & explosion).
  - Qi Refinement training field & Qi Refinement Boss dungeon.
  - Basic right-click movement code.

- In-progress / planned:
  - Auto-farm implementation.
  - Simple combat (hits, damage, death).
  - UI: HP/MP bars, Realm display, Internal Art selection.
  - Basic 2D assets for Sword Cultivator & mobs.

Post-MVP roadmap:

- More Internal Arts (Fire/Water/Body/Support paths).
- Higher realms (full Foundation Establishment, Golden Core, etc.).
- More maps, dungeons, and detailed gear/artifact systems.
- Cultivation features: meditation (AFK gains), alchemy, tribulations, etc.

---
