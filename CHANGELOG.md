# CHANGELOG

## Version 1.2.0 - 2025-10-28

### New Features
- Improved variety: now cycles through all available images before repeating
- Shuffle order is generated once per game session and repeats consistently
- You'll see all backgrounds before any repeats within a session

### Bug Fixes
- Fixed rare red/black screen appearing when switching game modes
- Corrected texture cleanup logic to prevent destroying game-owned textures
- Added null-texture filtering to prevent selection errors

### Technical
- Implemented Fisher-Yates shuffle-without-replacement algorithm
- Added timestamps to all debug log entries
- Enhanced debug logging to show shuffle order and position tracking

---

## Version 1.1.0 - 2025-10-27

### New Features
- Added persistent settings storage (debug logging preference now saves between sessions)

### Bug Fixes
- Fixed memory leak in RandomBackgroundProvider (materials were not being destroyed)
- Improved material lifecycle management

### Code Quality
- Replaced all magic numbers with named constants
- Renamed variables for improved readability

### Documentation
- Improved comments
- Added explanations for design decisions (e.g., red fallback color)

### Files Changed
- **NEW**: ModSettings.cs - XML-based settings persistence
- **MODIFIED**: Mod.cs - Added settings initialization and XML serialization
- **MODIFIED**: All .cs files updated with improved comments and variable names
- **MODIFIED**: CityscapeShuffle.csproj updated with better organization
- **MODIFIED**: Properties/AssemblyInfo.cs simplified and updated
- **NEW**: .gitignore, LICENSE, CHANGELOG.md, README.md


## Version 1.0.0 - 2025-09-20

Initial release (to Steam Workshop)
