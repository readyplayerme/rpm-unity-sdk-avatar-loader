This update includes a Quick Start sample scene, updates to the animation extractor, additional avatar loader component and a fix for caching. 

## Changelog

### Added
- Mesh Optimization compression support [#74](https://github.com/readyplayerme/rpm-unity-sdk-avatar-loader/pull/74)
- QueryBuilder class for handling Avatar API parameter generation [#71](https://github.com/readyplayerme/rpm-unity-sdk-avatar-loader/pull/71)
- EyeAnimationHandler and VoiceHandler now logs if required blendshapes are missing [#66](https://github.com/readyplayerme/rpm-unity-sdk-avatar-loader/pull/66)
- added extra unit tests for better coverage [#68]https://github.com/readyplayerme/rpm-unity-sdk-avatar-loader/pull/68()
- AvatarMetdata now includes a color hex value for SkinTone [#63](https://github.com/readyplayerme/rpm-unity-sdk-avatar-loader/pull/63)

### Fixed
- an issue caused by avatar URL's that have a space at beginning or end [#73](https://github.com/readyplayerme/rpm-unity-sdk-avatar-loader/pull/73)

### Updated
- AvatarRenderLoader now uses latest Render API via URL query parameters [#64](https://github.com/readyplayerme/rpm-unity-sdk-avatar-loader/pull/64)
- refactor of WebRequestDispatcher [#67](https://github.com/readyplayerme/rpm-unity-sdk-avatar-loader/pull/67)
- model urls for sample scenes updated [#72](https://github.com/readyplayerme/rpm-unity-sdk-avatar-loader/pull/72)
