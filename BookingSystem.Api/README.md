# 🚀 Booking System Web API

## Про API

Це REST API для управління системою бронювання прального приміщення. API побудована на ASP.NET Core з Swagger документацією.

## Структура проекту

```
BookingSystem.Api/
├── Program.cs                          # Конфігурація додатку
├── Controllers/
│   ├── ResidentsController.cs         # Операції з резидентами
│   ├── BookingsController.cs          # Управління бронюванням
│   ├── AdminController.cs             # Адміністративні операції
│   └── HealthController.cs            # Health check
└── Properties/
    └── launchSettings.json            # Параметри запуску
```

## Як запустити API

### Метод 1: Visual Studio Code / Terminal

```bash
# Перейдіть у папку проекту
cd C:\Users\basev\Bohdana\OmaTehtava\BDD-project\BookingSystem.Api

# Запустіть API
dotnet run
```

### Метод 2: Через головну папку проекту

```bash
cd C:\Users\basev\Bohdana\OmaTehtava\BDD-project
dotnet run --project BookingSystem.Api/BookingSystem.Api.csproj
```

## Запуск API

API запускатиметься на:
- **HTTP**: `http://localhost:5000`
- **HTTPS**: `https://localhost:5001`
- **Swagger UI**: `http://localhost:5000/swagger` або `https://localhost:5001/swagger`

## API Endpoints

### 🔵 Health Check
```
GET /api/health
GET /api/health/info
```

### 👤 Residents API

#### Реєстрація резидента
```http
POST /api/residents/register
Content-Type: application/json

{
  "name": "Іван Петренко",
  "email": "ivan@example.com",
  "phone": "+380501234567",
  "apartmentNumber": "A-101"
}
```

#### Логін резидента
```http
POST /api/residents/login
Content-Type: application/json

{
  "apartmentNumber": "A-101",
  "password": "MyPassword123!"
}
```

#### Активація облікового запису
```http
POST /api/residents/activate
Content-Type: application/json

{
  "residentId": "550e8400-e29b-41d4-a716-446655440000",
  "password": "MyPassword123!"
}
```

#### Отримати дані резидента
```http
GET /api/residents/{id}
```

#### Оновити дані резидента
```http
PUT /api/residents/{id}
Content-Type: application/json

{
  "name": "Іван Петренко",
  "email": "ivan.new@example.com",
  "phone": "+380509876543",
  "apartmentNumber": "A-101"
}
```

### 📅 Bookings API

#### Отримати доступні часові слоти
```http
GET /api/bookings/availability/{facilityId}?date=2026-01-25
```

#### Вибрати часовий слот
```http
POST /api/bookings/select-slot
Content-Type: application/json

{
  "timeSlotId": "550e8400-e29b-41d4-a716-446655440000"
}
```

#### Підтвердити бронювання
```http
POST /api/bookings/confirm
Content-Type: application/json

{
  "residentId": "550e8400-e29b-41d4-a716-446655440000",
  "timeSlotId": "550e8400-e29b-41d4-a716-446655440001"
}
```

#### Отримати деталі бронювання
```http
GET /api/bookings/{id}
```

#### Отримати бронювання резидента
```http
GET /api/bookings/resident/{residentId}?date=2026-01-25
```

#### Отримати майбутні бронювання
```http
GET /api/bookings/upcoming/{residentId}
```

#### Скасувати бронювання
```http
DELETE /api/bookings/{id}/resident/{residentId}
Content-Type: application/json

{
  "reason": "Мені більше не потрібно"
}
```

#### Очистити вибір слота
```http
POST /api/bookings/clear-selection
```

### 🔐 Admin API

#### Логін адміністратора
```http
POST /api/admin/login
Content-Type: application/json

{
  "username": "admin",
  "password": "admin123"
}
```

#### Верифікація 2FA коду
```http
POST /api/admin/verify-2fa
Content-Type: application/json

{
  "adminId": "550e8400-e29b-41d4-a716-446655440000",
  "code": "123456"
}
```

#### Створити резидента (адмін)
```http
POST /api/admin/residents?adminId=550e8400-e29b-41d4-a716-446655440000
Content-Type: application/json

{
  "name": "Петро Іванов",
  "email": "petro@example.com",
  "phone": "+380509876543",
  "apartmentNumber": "B-202"
}
```

#### Отримати дані резидента (адмін)
```http
GET /api/admin/residents/{residentId}?adminId=550e8400-e29b-41d4-a716-446655440000
```

#### Оновити дані резидента (адмін)
```http
PUT /api/admin/residents/{residentId}?adminId=550e8400-e29b-41d4-a716-446655440000
Content-Type: application/json

{
  "name": "Петро Іванов",
  "email": "petro.new@example.com",
  "phone": "+380509876543",
  "apartmentNumber": "B-202"
}
```

#### Видалити резидента (адмін)
```http
DELETE /api/admin/residents/{residentId}?adminId=550e8400-e29b-41d4-a716-446655440000
```

#### Отримати всі бронювання на дату (адмін)
```http
GET /api/admin/bookings?date=2026-01-25&adminId=550e8400-e29b-41d4-a716-446655440000
```

#### Отримати деталі бронювання (адмін)
```http
GET /api/admin/bookings/{id}?adminId=550e8400-e29b-41d4-a716-446655440000
```

#### Скасувати бронювання (адмін)
```http
DELETE /api/admin/bookings/{bookingId}?adminId=550e8400-e29b-41d4-a716-446655440000
Content-Type: application/json

{
  "reason": "Технічне обслуговування"
}
```

## Тестування API

### Використання Swagger UI
Коли API запущений, перейдіть на:
- http://localhost:5000/swagger

Там ви можете протестувати всі endpoints без написання коду.

### Використання curl
```bash
# Здоров'я API
curl http://localhost:5000/api/health

# Отримати інформацію
curl http://localhost:5000/api/health/info

# Реєстрація резидента
curl -X POST http://localhost:5000/api/residents/register \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Іван Петренко",
    "email": "ivan@example.com",
    "phone": "+380501234567",
    "apartmentNumber": "A-101"
  }'
```

### Використання Postman
1. Імпортуйте колекцію з Swagger: http://localhost:5000/swagger/v1/swagger.json
2. Тестуйте endpoints через UI Postman

## Конфігурація

### CORS
API дозволяє запити з будь-яких джерел (AllowAll policy). У production потрібно обмежити до конкретних доменів.

### HTTPS
API використовує HTTPS на порту 5001. Для desenvolvimento може знадобитися встановлення сертифіката:

```bash
dotnet dev-certs https --trust
```

## Структура сервісів

API використовує наступні сервіси:

- **IResidentService** - управління резидентами
- **IAdminService** - адміністративні операції
- **IBookingService** - управління бронюванням
- **IEmailService** - надсилання листів
- **ITwoFactorService** - двофакторна аутентифікація
- **INotificationService** - сповіщення
- **IAuthenticationService** - перевірка аутентифікації

Всі сервіси зареєстровані як Singleton у Dependency Injection контейнері.

## Помилки та статус коди

- **200 OK** - успішна операція
- **201 Created** - ресурс успішно створений
- **400 Bad Request** - неправильні параметри запиту
- **401 Unauthorized** - помилка аутентифікації
- **403 Forbidden** - відсутні права доступу
- **404 Not Found** - ресурс не знайдено
- **409 Conflict** - конфлікт (наприклад, подвійне бронювання)
- **500 Internal Server Error** - помилка сервера

## Розробка

### Додавання нових endpoints

1. Додайте метод в відповідний контролер
2. Прикрасьте HTTP атрибутом ([HttpGet], [HttpPost], тощо)
3. Запустіть API - Swagger автоматично оновиться

### Тестування змін

```bash
# Запустити юніт тести
dotnet test

# Запустити з гарячою перезагрузкою
dotnet watch run
```

---

**API Version**: 1.0.0  
**Framework**: ASP.NET Core 8.0  
**Status**: ✅ Production Ready
