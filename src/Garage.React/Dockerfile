FROM node:22 AS build

WORKDIR /app

COPY package.json package.json
COPY package-lock.json package-lock.json

RUN npm install

COPY . .

# Accept build arguments
ARG VITE_TEST_ENV

# Set them as environment variables for the build
ENV VITE_TEST_ENV=${VITE_TEST_ENV}

RUN npm run build

FROM nginx:alpine

COPY --from=build /app/default.conf.template /etc/nginx/templates/default.conf.template
COPY --from=build /app/dist /usr/share/nginx/html

# Expose the default nginx port
EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]
