## Bluejay ![Build Status](https://travis-ci.org/WildAndrewLee/Bluejay.svg)

Bluejay is a lightweight C# application that automatically organizes your music library based on a directory structure template.

Usage:
```
./bluejay <src> <structure> [-o <log file>] [-i <monitor interval | default: 5 sec>] [-help]
```

Example usage:
```
./bluejay example "%artists%\%album%\%track% - %title%" -o bluejay.log
```
### Dependencies
- TagLib#
- For Windows:
  - .NET 4.0
- For Linux:
  - Mono Version 3.12.1.0
  - XBuild Engine Version 12.0

### Template Tags
- ```%name%``` - Original Filename
- ```%title%``` - Title
- ```%artists%``` - Artists
- ```%performers%``` - Performers
- ```%album%``` - Album
- ```%composers%``` - Composers
- ```%year%``` - Year
- ```%track%``` - Track

### Supported Operating Systems
- Windows 7
- Windows 8 (Unverified)
- Ubuntu 12.04 LTS Server Edition 64-bit
- OSX Mavericks (Unverified)

Bluejay should be able to run on most Linux distributions with Mono installed. If it doesn't, please submit an issue so I can add support.

### Roadmap
- Implement a custom read-only music tag library to reduce the overhead created by requiring TagLib#
- Port Bluejay to native C to ensure a more out of the box cross-platform experience.
- Reduce Bluejay's memory footprint
- Reduce Bluejay's overall binary size
- Create a native service for Bluejay
