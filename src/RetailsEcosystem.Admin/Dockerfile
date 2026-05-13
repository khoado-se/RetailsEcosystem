FROM node:22-alpine AS build
WORKDIR /app
COPY package*.json ./
RUN npm ci
COPY . .
ARG VITE_HOST_URL=/api
ARG VITE_PRODUCT_PLACEHOLDER_IMAGE=http://localhost:8080/images/products/product-placeholder-image.jpg
ENV VITE_HOST_URL=$VITE_HOST_URL
ENV VITE_PRODUCT_PLACEHOLDER_IMAGE=$VITE_PRODUCT_PLACEHOLDER_IMAGE
RUN npm run build

FROM nginx:alpine AS runtime
COPY --from=build /app/dist /usr/share/nginx/html
COPY nginx.conf /etc/nginx/conf.d/default.conf
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
