#!/bin/bash

IMAGE=ghcr.io/natashalysakova/comic-shelf
read -p 'Version: ' VERSION

docker build -t ${IMAGE}:${VERSION} . -f ./ComicShelf/Dockerfile --platform linux/arm64 | tee build.log || exit 1
ID=$(tail -1 build.log | awk '{print $3;}')
docker tag $ID ${IMAGE}:latest

docker images | grep ${IMAGE}

docker image push ${IMAGE}:latest
docker image psuh ${IMAGE}:{$VERSION}

docker compose down
docker compose pull
docker compose up -d