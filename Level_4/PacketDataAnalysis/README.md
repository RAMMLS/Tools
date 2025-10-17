# Создаем контейнер 
docker build -t ufo-analysis .

# Запускаем контейнер 
docker run -it --rm ufo-analysis

# Если нужно сохранить графики на хост-машину
docker run -it --rm -v $(pwd)/output:/app/output ufo-analysis

# Запустите контейнер с монтированием
docker run -it --rm -v $(pwd)/ufo_graphs:/app/output ufo-analysis


# Запустите контейнер без --rm и в фоновом режиме
docker run -d --name ufo-container ufo-analysis

# Скопируйте файлы из контейнера
docker cp ufo-container:/app/countries_plot.png ./
docker cp ufo-container:/app/months_plot.png ./
docker cp ufo-container:/app/shapes_plot.png ./
docker cp ufo-container:/app/durations_plot.png ./

# Удалите контейнер
docker rm -f ufo-container
