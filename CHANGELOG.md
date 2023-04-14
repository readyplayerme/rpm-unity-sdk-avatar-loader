# Changelog

All notable changes to this project will be documented in this file.
This project adheres to [Semantic Versioning](http://semver.org/).

## [1.2.0] - 2023.04.14

### Added
- Mesh Optimization compression support.

## [1.1.0] - 2023.03.21

### Added
- quick start sample
- animation extract now supports extracting multiple files at once
- avatar loaded events
- avatar component

### Updated
- animation extractor path

### Fixed
- caching issue related to time zone differences

## [1.0.0] - 2023.02.20

### Added
- support for offline avatar loading from cache
- optional sdk logging
- glTF fast defer agent support
- texture channel support for avatar config

### Updated
- PartnerSubdomainSettings refactored to a CoreSettings scriptable object

### Fixed
- Added missing URP shader variant
- core settings asset now automatically created if it is missing.
- Various other bug fixes and improvements

## [0.2.0] - 2023.02.08

### Added
- support for offline avatar loading from cache
- optional sdk logging
- glTF fast defer agent support
- texture channel support for avatar config

### Updated
- PartnerSubdomainSettings refactored to a CoreSettings scriptable object

### Fixed
- Added missing URP shader variant
- Various other bug fixes and improvements

## [0.1.1] - 2023.01.22

### Added
- missing shader variant for URP shader variant collection

## [0.1.0] - 2023.01.12

### Added
- inline code documentation
- Contribution guide and code of conduct
- Added samples in optional samples folders
- GLTF/GLB files now use gltFast importer
- shader variant helper to check and import missing shaders

### Updated
- A big refactor of code and classes

### Fixed
- Various bug fixes and improvements