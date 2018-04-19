### Создание контейнера / докеризация приложения

Рассмотрим, как можно создать docker-контейнер, в котором будет работать flask-приложение. Потом сделаем приложение доступным через nginx-сервер, работающий в отдельном контейнере.

##### Создание Docker-файла

```
FROM python:2.7

RUN mkdir -p /code
COPY . /code
VOLUME [ "/code" ]
WORKDIR /code
RUN pip install -r requirements.txt
EXPOSE 5000
CMD [ "python", "/code/app.py" ]
```

В текущем каталоге должны быть размещены:

**app.py**

```
from flask import Flask
from redis import Redis
import os
app = Flask(__name__)
redis = Redis(host='redis', port=6379)

@app.route('/')
def hello():
    redis.incr('hits')
    return 'Hello World! I have been seen %s times.' % redis.get('hits')

if __name__ == "__main__":
    app.run(host="0.0.0.0", debug=True)
```

**requirements.txt**

```
flask
redis
```

##### Сборка образа

```
docker build -t flaskapp .
```
##### Запуск контейнера с новым образом

```
docker run -d -P --name flaskapp flaskapp
```

Просмотреть, работает ли контейнер, и на каком порту он доступен:

```
 $ sudo docker ps
 CONTAINER ID        IMAGE               COMMAND                CREATED             STATUS              PORTS                     NAMES
 0c8339084e07        flaskapp:latest     "python /code/app.py   5 seconds ago       Up 3 seconds        0.0.0.0:49154->5000/tcp   flaskapp            
```

Если обратиться на порт 49154 хост-системы, будет получен доступ к приложению, работающему внутри контейнера.

Поскольку наше приложение использует для своей работы внешний сервис (redis), нам потребуется контейнер с redis, который будет подключен к этому контейнеру.

```
docker rm -f flaskapp
docker run -d --name redis redis
docker run -d -P --name flaskapp --link redis:redis flaskapp
```

Теперь нашему приложению доступен Redis-сервер.

##### Подключить дополнительные образы

При необходимости можно подключить дополнительные образы Docker, создать связку контейнеров.

Создадим nginx-контейнер, который является сетевым фронтендом для flask-приложения.

Создадим конфигурационный файл nginx назовём flaskapp.conf:

```
server {
    listen 80;

    location / {
        proxy_pass http://flaskapp:5000;
    }
}
```

Создадим Dockerfile:

```
FROM nginx:1.7.8
COPY flaskapp.conf /etc/nginx/conf.d/default.conf
```

После этого нужно построить и запустить образ:

```
docker build -t nginx-flask .
docker run --name nginx-flask --link flaskapp:flaskapp -d -p 8080:80 nginx-flask
```

Сейчас работает три контейнера, которые взаимосвязаны друг с другом:

```
      +-------+      +-------+      +-------+
  8080|       |  5000|       |      |       |
      o nginx +----->o flask +----->| redis |
      |       |      |       |      |       |
      +-------+      +-------+      +-------+
```

Работающие контейнеры:

```
$ docker ps
CONTAINER ID        IMAGE                COMMAND                CREATED             STATUS              PORTS                           NAMES
980b4cb3002a        nginx-flask:latest   "nginx -g 'daemon of   59 minutes ago      Up 59 minutes       443/tcp, 0.0.0.0:8080->80/tcp   nginx-flask         
ae4320dc419a        flaskapp:latest      "python /code/app.py   About an hour ago   Up About an hour    0.0.0.0:49161->5000/tcp         flaskapp            
3ecaab497403        redis:latest         "/entrypoint.sh redi   About an hour ago   Up About an hour    6379/tcp                        redis     
```

Проверим, отвечает ли наш сервис:

```
$ curl http://5.9.243.189:8080/ ; echo
Hello World! I have been seen 1 times.

$ curl http://5.9.243.189:8080/ ; echo
Hello World! I have been seen 2 times.

$ curl http://5.9.243.189:8080/ ; echo
Hello World! I have been seen 3 times.
```


