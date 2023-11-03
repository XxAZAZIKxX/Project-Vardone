<picture>
  <source media="(prefers-color-scheme: dark)" srcset="https://i.imgur.com/XFnSrVk.png">
  <source media="(prefers-color-scheme: light)" srcset="">
  <img>
</picture>

[Russian version of README](https://github.com/XxAZAZIKxX/Project-Vardone/blob/master/README_ru-RU.md)

# Vardone
## Description
Vardone - an application for instant messaging between users.

This repository is composed of:
- [VardoneAPI](https://github.com/XxAZAZIKxX/Project-Vardone/tree/master/VardoneApi) - server API application that handles all application requests
- [Vardone](https://github.com/XxAZAZIKxX/Project-Vardone/tree/master/Vardone) - WPF application that is used by the end user
- [VardoneLibrary](https://github.com/XxAZAZIKxX/Project-Vardone/tree/master/VardoneLibrary) - library which has ready methods for interaction with Web API

## App Features
- Add/delete friends
- Create/edit/delete servers
- Add/Rename/Delete channels on a server
- Send images in a message
- Complain about other messages
- Edit your profile and personal information
- Delete your account or log out of your account from all devices

## Technologies Used
The technologies used in the development of Vardone were:
- ASP.NET Web API
- Rest API
- WPF
- EntityFramework
- JWT tokens

## About data storage
Passwords are stored in MD5 format. Users' personal data, such as full name, phone number and others, are encrypted using AES-256. This data is stored in a MySQL database.

### A little bit of the program's appearance
- ### Channel chat on the server
![v1](https://i.imgur.com/vA9Tvpw.jpg)
- ### Friends list
![v2](https://i.imgur.com/aAaCRWG.jpg)
- ### Profile settings
![v3](https://i.imgur.com/yHXNulZ.jpg)
- ### Sending messages in private chat rooms
![v4](https://i.imgur.com/zDQmSXi.png)
