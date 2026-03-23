# AVA Elite

A faithful recreation of the classic 1984 **Elite** game by David Braben and Ian Bell, built with .NET 9 and Avalonia UI. Features authentic wireframe 3D graphics with hidden line removal.

## Screenshots

The game renders all ships, stations, and planets as green wireframe models on a black background, just like the BBC Micro original.

## Features

### 3D Wireframe Engine
- Back-face culling via face normal dot product for hidden line removal
- Perspective projection with convex hull ship models
- Edges drawn only when at least one adjacent face points toward the camera
- No external 3D libraries — pure `DrawingContext` line rendering

### Ships
Cobra Mk III, Sidewinder, Viper, Mamba, Krait, Asp Mk II, Python, Thargoid, Coriolis Station, Missile, Asteroid, Cargo Canister, Escape Pod

### Gameplay
- **Trading** — 17 commodities with supply/demand economics across 256 star systems
- **Combat** — 4 laser types, homing missiles, ECM, energy bomb
- **NPC AI** — pirates, traders, police, bounty hunters, Thargoids
- **Navigation** — galaxy map, short range chart, hyperspace jumps across 8 galaxies
- **Equipment** — 16 purchasable upgrades (lasers, fuel scoops, docking computer, etc.)
- **Procedural universe** — seed-based galaxy generation matching the original algorithm
- **Legal system** — Clean / Offender / Fugitive status
- **Combat rating** — Harmless through Elite

## Controls

| Key | Action |
|-----|--------|
| W / S | Pitch up / down |
| , / . | Roll left / right |
| Z / X | Speed up / slow down |
| Space | Fire lasers |
| T | Target missile |
| M | Fire missile |
| E | ECM (if equipped) |
| B | Energy bomb (if equipped) |
| 1-4 | Front / Rear / Left / Right view |
| J | Hyperspace jump |
| H | Galactic hyperspace (if equipped) |
| F1 | Launch / Flight |
| F4 | Market (when docked) |
| F5 | Commander status |
| F6 | Galaxy map |
| F7 | Short range chart |
| F8 | System data |
| F9 | Equipment (when docked) |

## Requirements

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)

## Build & Run

```bash
cd AVAElite
dotnet run
```

## Project Structure

```
AVAElite/
  Core/        — Game loop, input, math (Vector3/Matrix3x3), seed RNG
  Models/      — Ship wireframe data (vertices, edges, faces)
  Universe/    — Galaxy generation, star systems, market, commodities
  Flight/      — Space entities, scene management, NPC AI
  Combat/      — Weapons, damage (integrated into flight screen)
  Rendering/   — 3D renderer with hidden line removal, HUD, scanner
  Screens/     — Title, flight, maps, market, status, equipment
  Views/       — Avalonia GameCanvas control
```

## Credits

- Original game: David Braben and Ian Bell (1984)
- Avalonia recreation: Built with Claude Code
