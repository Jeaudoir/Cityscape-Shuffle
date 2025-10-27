# CHANGELOG

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