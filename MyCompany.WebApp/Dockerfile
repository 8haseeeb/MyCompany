# Build Stage
FROM node:20-alpine AS build
WORKDIR /app

# Copy root package files
COPY package.json package-lock.json ./

# Copy all workspace package files
COPY apps/host/package.json ./apps/host/
COPY apps/promotions/package.json ./apps/promotions/

# Install dependencies
RUN npm install

# Copy source code
COPY . .

# Build both apps
RUN npm run build --workspace=apps/host
RUN npm run build --workspace=apps/promotions

# Production Stage
FROM nginx:stable-alpine
WORKDIR /usr/share/nginx/html

# Remove default nginx static assets
RUN rm -rf ./*

# Copy build outputs from build stage
# Assuming Vite build output goes to dist/
COPY --from=build /app/apps/host/dist ./host
COPY --from=build /app/apps/promotions/dist ./promotions

# Copy custom nginx config as a template
COPY nginx.conf /etc/nginx/conf.d/default.conf.template

EXPOSE 80


# Use envsubst to replace variables in nginx config at startup
CMD ["/bin/sh", "-c", "envsubst '${GATEWAY_URL} ${GATEWAY_HOST}' < /etc/nginx/conf.d/default.conf.template > /etc/nginx/conf.d/default.conf && nginx -g 'daemon off;'"]




