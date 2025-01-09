#!/bin/bash

# Exit immediately if a command exits with a non-zero status
set -e

IMAGE=ghcr.io/natashalysakova/comic-shelf
read -p 'Version: ' VERSION

docker build -t ${IMAGE}:${VERSION} . -f ./ComicShelf/Dockerfile --platform linux/arm64 | tee build.log || exit 1
docker tag ${IMAGE}:${VERSION} ${IMAGE}:latest

docker images | grep ${IMAGE}

docker image push ${IMAGE}:latest
docker image push ${IMAGE}:${VERSION}

docker compose down
docker compose pull
docker compose up -d