# syntax=docker/dockerfile:1
FROM ubuntu:noble AS build-pdf2htmlex

# Produces a patched pdf2htmlEX using libopenjp 2.7 instead of libjpeg to get JPEG2000 support.

WORKDIR /source
RUN apt update && apt install -y git patch sudo
RUN git clone --depth=1 --branch fbfa07fdc1fd14bee2cb6f4c0f5f85e017bc6614 https://github.com/CoreFiling/pdf2htmlEX
WORKDIR /source/pdf2htmlEX

RUN ./buildScripts/buildInstallLocallyApt
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
RUN apt install -y libjpeg62 libopenjp2-7 /pdf2htmlEX/pdf2htmlEX-*-x86_64.deb

WORKDIR /app
COPY --from=build /app ./
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
CMD ["./Pdf2Html"]