# Changelog

## develop

* Update to .net 8.
  * Switch base images to Ubuntu Noble (24.04 LTS).
* Patch and build `pdf2htmlEX` as part of this build process to use `libopenjp` instead of `libjpeg` for JPEG-2000 support.
  * All patches are in this source tree, and are applied to directly to the source of the upstream tag during build.
* Patch issue with non-breaking spaces in `pdf2HTMLEX`.
* Convert complex SVGs images to bitmaps.

## 0.1.0

* Initial release.
