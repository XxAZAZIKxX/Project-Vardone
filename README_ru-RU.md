<picture>
  <source media="(prefers-color-scheme: dark)" srcset="https://i.imgur.com/XFnSrVk.png">
  <source media="(prefers-color-scheme: light)" srcset="">
  <img>
</picture>

# Vardone
## Описание

Vardone - это приложение для мгновенного обмена сообщениями между пользователями.

Данный репозиторий состоит из:
- [VardoneAPI](https://github.com/XxAZAZIKxX/Project-Vardone/tree/master/VardoneApi) - серверное API приложение которое обрабатывает все запросы приложения
- [Vardone](https://github.com/XxAZAZIKxX/Project-Vardone/tree/master/Vardone) - WPF приложение которое используется конечным пользователем
- [VardoneLibrary](https://github.com/XxAZAZIKxX/Project-Vardone/tree/master/VardoneLibrary) - библиотека которая имеет готовые методы для взаимодействия с Web API

## Возможности приложения
- Добавлять/удалять друзей
- Создавать/редактировать/удалять сервера
- Добавлять/Переименовывать/удалять каналы на сервере
- Отправлять изображения в сообщении
- Жаловаться на другие сообщения
- Редактировать свой профиль и личные данные
- Удалить свой аккаунт или выйти с аккаунта со всех устройств

## Использованные технологии
В разработке Vardone были использованы технологии:
- ASP.NET Web API
- Rest API
- WPF
- EntityFramework
- JWT токены

## О хранении данных
Пароли сохранены в формате MD5. Личные данные пользователей, такие как ФИО, номер телефона и прочие, зашифрованы с использованием AES-256. Эти данные хранятся в базе данных MySQL.

## Немного внешнего вида программы
- ### Чат канала на сервере
![v1](https://i.imgur.com/vA9Tvpw.jpg)
- ### Список друзей
![v2](https://i.imgur.com/aAaCRWG.jpg)
- ### Настройки профиля
![v3](https://i.imgur.com/yHXNulZ.jpg)
- ### Отправка сообщений в приватных чатах
![v4](https://i.imgur.com/zDQmSXi.png)
