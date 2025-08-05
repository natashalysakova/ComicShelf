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

docker exec -ti shelf-backend /bin/bash

apt-get update \
    && apt-get install -y wget gnupg \
    && wget -q -O - https://dl-ssl.google.com/linux/linux_signing_key.pub | apt-key add - \
    && sh -c 'echo "deb [arch=amd64] http://dl.google.com/linux/chrome/deb/ stable main" >> /etc/apt/sources.list.d/google.list' \
    && apt-get update \
    && apt-get install -y google-chrome-stable fonts-ipafont-gothic fonts-wqy-zenhei fonts-thai-tlwg fonts-kacst fonts-freefont-ttf libxss1 \
      --no-install-recommends \
    && rm -rf /var/lib/apt/lists/*