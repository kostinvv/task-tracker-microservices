# ☑️ Task Tracker
Персональный планировщик задач на основе Kanban-доски

## Использованные технологии
- ✔️ .NET 9 & ASP.NET Core;
- ✔️ ASP.NET Core;
- ✔️ Confluent.Kafka;
- ✔️ Entity Framework Core;
- ✔️ Swagger & Swagger UI;
- ✔️ MailKit;
- ✔️ Quartz;
- ✔️ JwtBearer;
- ✔️ Identity;
- ✔️ React SPA;
- ✔️ Docker & Docker compose;
- ✔️ CI/CD & GitHub Actions.

## Сервисы:

### Tasks
Предоставляет REST API для пользователей, авторизации и задач. 

Описание API эндпоинтов:

**Tasks:**
- **GET** /api/v1/tasks/board - board-представление;
- **GET** /api/v1/tasks - получить список задач;
- **POST** /api/v1/tasks - создать задачу;
- **GET** /api/v1/tasks/{id} - получить задачу по id;
- **PUT** /api/v1/tasks/{id} - обновить задачу;
- **DELETE** /api/v1/tasks/{id} - удалить задачу;
- **PATCH** /api/v1/tasks/{id}/move - изменить позицию на доске.

**User:** 
- **GET** /api/v1/user - получить текущего пользователя;
- **POST** /api/v1/user - регистрация пользователя;
- **POST** /api/v1/user/login - логин;

### EmailSender
Читает сообщения из Kafka-топика EMAIL_SENDING_TASKS и отправляет письма через SMTP.
Для отправки используется Mailkit, а интеграция с брокером - через Confluent.Kafka.

### Scheduler
По расписанию (Cron) формирует суточные отчеты по задачам пользователей.

### Frontend
SPA на React, предоставляющее пользовательский интерфейс для работы с Task Tracker.

Основные возможности:
- формы входа и регистрации;
- основной экран в виде Kanban-доски;
- поддержка drag-and-drop (перетаскивание карточек между колонками и изменение порядка) для управления задачами в стиле Trello.

## CI/CD
C помощью GitHub Actions была автоматизирована сборка Docker образов и отправка в публичный репозиторий Dockerhub.

## Настройка и запуск

В репозитории, в папке docker-compose, лежат sample .env-файлы конфигурации для каждого сервиса. Перед запуском скопируйте sample-файлы и заполните значения переменных окружения (хосты, порты, логины/пароли, секреты и т.д.).

#### TaskTracker - пример .env

``` text
BaseUrl=http://localhost:8080/

# DB
PostgresHostPort=
PostgresPort=
PostgresUser=
PostgresPassword=

JsClient=http://localhost:4173

# JWT
Jwt__SecretKey=
Jwt__Issuer=
Jwt__Audience=
Jwt__ExpiresHours=

# Kafka
Kafka__Tasks__BootstrapServers=broker-1:9092
Kafka__Tasks__Topic=EMAIL_SENDING_TASKS
Kafka__GroupId__Topic=
```
- В секции DB задаются параметры подключения к PostgreSQL
- BaseUrl и JsClient - адреса backend и клиентского приложения
- В секции JWT указываются параметры генерации токенов
- В секции Kafka указываются настройки брокера/топика и group id

#### Scheduler - конфигурация
У сервиса Scheduler конфигурация аналогична (DB/Kafka), дополнительно можно настроить расписание выполнения через Quartz:
``` text
Quartz__CronSchedule=0 * * ? * *
```

#### EmailSender - конфигурация
Для сервиса EmailSender дополнительно требуется настроить SMTP-клиент:
``` text
# SMTP Client
Smtp__Host=
Smtp__Port=
Smtp__UserName=
Smtp__Password=
```

#### Запуск приложения
Приложение запускается через docker-compose.yaml, расположенный в папке docker-compose.

`docker compose up -d`