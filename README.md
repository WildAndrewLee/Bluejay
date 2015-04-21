## Bluejay

Bluejay is a lightweight C# application that automatically organizes your music library based on a directory structure template.

Example usage:
```
./bluejay example "%artists%\%album%\%track% - %title%" -o bluejay.log
```
### Dependencies
- TagLib#

### Template Tags
- ```%name%``` - Original Filename
- ```%title%``` - Title
- ```%artists%``` - Artists
- ```%performers%``` - Performers
- ```%album%``` - Album
- ```%composers%``` - Composers
- ```%year%``` - Year
- ```%track%``` - Track
