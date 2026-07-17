# Build
FROM node:24-alpine AS build
WORKDIR /source

COPY package.json package-lock.json ./
RUN npm ci

COPY . .
RUN npm run build

# NGINX
FROM nginx:alpine AS final
COPY nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /source/dist/haushalts-os/browser /usr/share/nginx/html
EXPOSE 80