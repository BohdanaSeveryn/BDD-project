# Облікові записи системи бронювання

## Адміністратор

**Username:** `admin`  
**Password:** `Admin@123`  
**Email:** admin@bookingsystem.fi  
**Доступ:** Адміністративна панель за адресою http://localhost:5000/admin-login.html

---

## Жителі

### Житель 1: Matti Virtanen

**Apartment Number:** `A101`  
**Password:** `Matti@123`  
**Email:** matti.virtanen@example.com  
**Phone:** +358401234567  
**Доступ:** Головна сторінка http://localhost:5000

### Житель 2: Liisa Korhonen

**Apartment Number:** `B205`  
**Password:** `Liisa@123`  
**Email:** liisa.korhonen@example.com  
**Phone:** +358407654321  
**Доступ:** Головна сторінка http://localhost:5000

---

## Примітки

- Всі облікові записи створюються автоматично при першому запуску програми
- Для входу жителів використовується номер квартири (Apartment Number) замість username
- Адмін має окрему сторінку для входу
- Паролі захищені за допомогою BCrypt хешування
