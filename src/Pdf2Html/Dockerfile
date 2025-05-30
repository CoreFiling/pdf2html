# syntax=docker/dockerfile:1
FROM ubuntu:noble AS build-pdf2htmlex

# Produces a patched pdf2htmlEX using libopenjp 2.7 instead of libjpeg to get JPEG2000 support.

ENV PDF2HTMLEX_BRANCH=
ENV UNATTENDED="--assume-yes"
ENV MAKE_PARALLEL="-j 4"
ENV PDF2HTMLEX_PREFIX=/usr/local
ENV DEBIAN_FRONTEND=noninteractive

WORKDIR /source
RUN apt update && apt install -y git patch sudo
RUN git clone --depth=1 --branch v0.18.8.rc1 https://github.com/pdf2htmlEX/pdf2htmlEX
WORKDIR /source/pdf2htmlEX

COPY ./pdf2htmlEX/patches ./patches
RUN patch ./buildScripts/versionEnvs ./patches/versionEnvs.patch
RUN patch ./buildScripts/buildPoppler ./patches/buildPoppler.patch
RUN patch ./buildScripts/getBuildToolsApt ./patches/getBuildToolsApt.patch
RUN patch ./buildScripts/getDevLibrariesApt ./patches/getDevLibrariesApt.patch
RUN patch ./pdf2htmlEX/src/BackgroundRenderer/SplashBackgroundRenderer.cc ./patches/SplashBackgroundRenderer.cc.patch
RUN patch ./pdf2htmlEX/src/util/unicode.cc ./patches/unicode.cc.patch
RUN patch ./pdf2htmlEX/src/util/unicode.h ./patches/unicode.h.patch
RUN patch ./pdf2htmlEX/CMakeLists.txt ./patches/CMakeLists.patch

RUN ./buildScripts/versionEnvs
RUN ./buildScripts/reportEnvs
RUN ./buildScripts/getBuildToolsApt
RUN ./buildScripts/getDevLibrariesApt
RUN ./buildScripts/getPoppler
RUN patch ./poppler/glib/poppler-enums.c.template ./patches/poppler-enums.c.template.patch
RUN patch ./poppler/glib/poppler-private.h ./patches/poppler-private.h.patch
RUN ./buildScripts/buildPoppler
RUN ./buildScripts/getFontforge
RUN patch ./fontforge/fontforge/tottfgpos.c ./patches/fontforge-tottfgpos.c.patch
RUN ./buildScripts/buildFontforge
RUN ./buildScripts/buildPdf2htmlEX
RUN ./buildScripts/installPdf2htmlEX
RUN git config user.name "CoreFiling"
RUN git config user.email "opensource@corefiling.com"
RUN ./buildScripts/createDebianPackage

FROM mcr.microsoft.com/dotnet/sdk:8.0-noble AS build
WORKDIR /source
COPY ./ .
RUN dotnet publish -c release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0-noble
RUN apt update && apt install -y wget
RUN wget http://archive.ubuntu.com/ubuntu/pool/main/libj/libjpeg-turbo/libjpeg-turbo8_2.0.3-0ubuntu1_amd64.deb
RUN apt install -y ./libjpeg-turbo8_2.0.3-0ubuntu1_amd64.deb
COPY --from=build-pdf2htmlex /source/pdf2htmlEX/imageBuild/*.deb /pdf2htmlEX/
RUN apt install -y libjpeg62 libopenjp2-7 /pdf2htmlEX/pdf2htmlEX-0.18.8.rc1-cfl2-*-x86_64.deb

WORKDIR /app
COPY --from=build /app ./
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
CMD ["./Pdf2Html"]