# Changelog

## develop

* Update to .net 8.
  * Switch base images to Ubuntu Noble (24.04 LTS).
* Increase `font-size-multiplier` to increase text rendering fidelity and get rid of sporadic empty spaces at the end of numbers.
* Patch and build `pdf2htmlEX` as part of this build process to use `libopenjp` instead of `libjpeg` for JPEG-2000 support.
  * All patches are in this source tree, and are applied to directly to the source of the upstream tag during build.

## 0.1.0

* Initial release.
