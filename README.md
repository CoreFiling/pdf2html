# pdf2html

This project is a lightweight HTTP(S) interface to the [pdf2htmlex library](https://pdf2htmlex.github.io/pdf2htmlEX/).

## Running via Docker

```bash
docker run -p 8080 corefiling/pdf2html:$version
```

### Overriding `pdf2htmlEX` options

The command line arguments passed into `pdf2htmlEX` can be overridden by passing in environment variables prefixed by `ConversionOptions__`, e.g:

```bash
docker run -p 8080 -e ConversionOptions__BgFormat=png -e ConversionOptions__OptimizeText=true corefiling/pdf2html$version
```

The names of these setting keys are converted to lower-kebab-case arguments, and the values are converted to strings as needed - in the above example, the arguments are converted to `--bg-format=png --optimize-text=0`.

The full list of arguments can be found by running `pdf2htmlEX`:

```bash
docker run corefiling/pdf2html pdf2htmlEX:$version --help
```

## Licensing

Since pdf2htmlex is licensed under the GPL, this project is too (see the LICENSE.TXT file).

As you can see from the build process, pdf2htmlEX itself is patched by the patches within this project (see [src/Pdf2Html/patches](tree/src/Pdf2Html/patches)), based on a clone of the upstream project tag we are targeting. As such we have not repeated pdf2htmlEX's source code here; you can find it via the link above.
