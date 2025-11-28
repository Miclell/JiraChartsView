# JiraChartsView

![Tests](https://github.com/Miclell/JiraChartsView/actions/workflows/tests.yml/badge.svg)

Веб-приложение для визуализации аналитики задач Apache Jira.

## Возможности

- **Гистограмма времени открытых задач** - отражает время, которое задача провела в открытом состоянии
**Распределение времени по статусам** - показывает распределение времени по состояниям задач
- **Ежедневный поток задач** - количество созданных и закрытых задач в день с накопительным итогом
- **Топ-30 пользователей** - рейтинг пользователей по количеству задач (репортер + исполнитель)
- **Гистограмма времени жизни задач** - время от создания до закрытия
- **Распределение по приоритетам** - количество задач по степени серьезности

## Архитектура

### Backend (.NET 9.0)
- **Clean Architecture**:
  - `Core` - доменные модели и интерфейсы
  - `Application` - бизнес-логика и CQRS + MediatR
  - `Infrastructure` - интеграция с Jira API
  - `Web` - REST API контроллеры

### Frontend (React + TypeScript)
- React 18 с TypeScript
- Tailwind CSS для стилизации
- Recharts для визуализации данных
- Vite для быстрой сборки

## Установка и запуск

### Требования
- .NET 9.0 SDK
- Node.js 20.x или выше

### Backend

```bash
dotnet restore

dotnet run --project src/Presentation/Web/Web.csproj
```

Будет запущен на http://localhost:5085

### Frontend

```bash
cd src/Presentation/Client

npm install

npm run dev
```

Приложение будет доступно по адресу http://localhost:5173

### Docker

```bash
cd ops

# Сконфигурируйте .env

docker-compose up -d
```

___

[Лицензия](LICENSE)

