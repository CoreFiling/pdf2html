# Changelog

## 0.3.0

* Switch to `cfl3` CFL patched fork instead of patching as part of this build.
  * This improves support for certain font CMaps
  * Remove `--tounicode` in favour of `--ignore-tounicode`, as `force` is no longer required.

## 0.2.2

* Patch memory corruption bug due to PNG background images being the incorrect size.
* Patch memory corruption bug in `fontforge` triggered by some fonts.

## 0.2.1

* Process PDF annotations (e.g. stamps) by default.

## 0.2.0

* Update to .net 8.
  * Switch base images to Ubuntu Noble (24.04 LTS).
* Add optional overrides for command-line arguments passed to `pdf2htmlEX`.
* Patch and build `pdf2htmlEX` as part of this build process to use `libopenjp` instead of `libjpeg` for JPEG-2000 support.
  * All patches are in this source tree, and are applied to directly to the source of the upstream tag during build.
* Patch issue with non-breaking spaces and tab characters in `pdf2HTMLEX`.
* Convert complex SVGs images to bitmaps.

## 0.1.0

* Initial release.
