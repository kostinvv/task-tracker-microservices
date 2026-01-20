FROM node:20-slim
WORKDIR /app

COPY src/UIs/Spa/react-task-tracker/package*.json ./
RUN npm ci

COPY src/UIs/Spa/react-task-tracker/ .

ARG API_URL

ENV VITE_API_URL=$API_URL

RUN npm run build

EXPOSE 4173

CMD ["npm", "run", "preview", "--", "--host", "--port", "4173"]