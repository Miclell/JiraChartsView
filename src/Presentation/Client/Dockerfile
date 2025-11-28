FROM node:18-alpine as builder
ARG API_BASE_URL=/api/jira-analytics

WORKDIR /app
ENV VITE_API_BASE_URL=${API_BASE_URL}

COPY package*.json ./
COPY tsconfig*.json ./
COPY vite.config.ts ./
COPY tailwind.config.js ./
COPY postcss.config.js ./

RUN npm install

COPY . .

RUN npm run build

FROM node:18-alpine

WORKDIR /app

# Устанавливаем serve (лучше чем http-server для SPA)
RUN npm install -g serve

COPY --from=builder /app/dist ./dist

EXPOSE 8080

# Serve для SPA (автоматически redirect на index.html для роутинга)
CMD ["serve", "-s", "dist", "-l", "8080"]