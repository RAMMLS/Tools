import pandas as pd 
import numpy as np 
import pycountry 
import matplotlib.pyplot as plt 
import os

# Создаем папку для графиков
os.makedirs('/app/ufo_graphs', exist_ok=True)

# Размер надписей на графиках 
PLOT_LABEL_FONT_SIZE = 14

# Генерация цветовой схемы 
def getColors(n):
    COLORS = []
    # Исправлено: новая версия get_cmap
    cmap = plt.colormaps['hsv'].resampled(n)

    for i in np.arange(n):
        COLORS.append(cmap(i))

    return COLORS

# Заглушка для перевода
def translate(string, translator_obj=None):
    translations = {
        'circle': 'круг',
        'triangle': 'треугольник', 
        'fireball': 'огненный шар',
        'oval': 'овал',
        'light': 'свет',
        'sphere': 'сфера',
        'unknown': 'неизвестно',
        'United States': 'США',
        'Canada': 'Канада'
    }
    return translations.get(string, string)

# Сортировка объектов 
def dict_sort(my_dict):
    keys = []
    values = []
    my_dict = sorted(my_dict.items(), key=lambda x:x[1], reverse=True)
    for k, v in my_dict:
        keys.append(k)
        values.append(v)
    return (keys, values)

# Загрузка данных
try:
    df = pd.read_csv('./scrubbed.csv', low_memory=False)
    df['shape'] = df['shape'].fillna('unknown')
except Exception as e:
    print(f"Ошибка загрузки CSV: {e}")
    exit(1)

# Обработка стран
country_label_count = pd.Series(df['country'].values).value_counts()
for label in list(country_label_count.keys()):
    try:
        if str(label).upper() == 'US':
            df = df.replace({'country': str(label)}, 'США')
        elif str(label).upper() == 'CA':
            df = df.replace({'country': str(label)}, 'Канада')
        else:
            c = pycountry.countries.get(alpha_2=str(label).upper())
            if c:
                t = translate(c.name)
                df = df.replace({'country': str(label)}, t)
    except Exception as e:
        print(f"Ошибка обработки страны {label}: {e}")

# Перевод названий форм
shapes_label_count = pd.Series(df['shape'].values).value_counts()
for label in list(shapes_label_count.keys()):
    t = translate(str(label))
    df = df.replace({'shape': str(label)}, t)

# График 1: Страны с наибольшим количеством наблюдений
country_count = pd.Series(df['country'].values).value_counts(sort=True)
country_count_keys, country_count_values = dict_sort(dict(country_count))    
TOP_COUNTRY = min(len(country_count_keys), 10)

plt.figure(figsize=(12, 6))
plt.title('Страны, где больше всего наблюдений', fontsize=PLOT_LABEL_FONT_SIZE)
plt.bar(np.arange(TOP_COUNTRY), country_count_values[:TOP_COUNTRY], color=getColors(TOP_COUNTRY))
plt.xticks(np.arange(TOP_COUNTRY), country_count_keys[:TOP_COUNTRY], rotation=45, fontsize=12)
plt.yticks(fontsize=PLOT_LABEL_FONT_SIZE)
plt.ylabel('Количество наблюдений', fontsize=PLOT_LABEL_FONT_SIZE)
plt.tight_layout()
plt.savefig('/app/ufo_graphs/countries_plot.png')
print("График стран сохранен: /app/ufo_graphs/countries_plot.png")
plt.show()

# График 2: Наблюдения по месяцам
MONTH_COUNT = [0,0,0,0,0,0,0,0,0,0,0,0]
MONTH_LABEL = ['Январь', 'Февраль', 'Март', 'Апрель', 'Май', 'Июнь',
    'Июль', 'Август', 'Сентябрь' ,'Октябрь' ,'Ноябрь' ,'Декабрь']

for i in df['datetime']:
    try:
        parts = str(i).split('/')
        if len(parts) >= 2:
            m = parts[0]
            if m.isdigit() and 1 <= int(m) <= 12:
                MONTH_COUNT[int(m)-1] += 1
    except:
        continue

plt.figure(figsize=(12, 6))
plt.bar(np.arange(12), MONTH_COUNT, color=getColors(12))
plt.xticks(np.arange(12), MONTH_LABEL, rotation=45, fontsize=PLOT_LABEL_FONT_SIZE)
plt.ylabel('Частота появления', fontsize=PLOT_LABEL_FONT_SIZE)
plt.yticks(fontsize=PLOT_LABEL_FONT_SIZE)
plt.title('Частота появления объектов по месяцам', fontsize=PLOT_LABEL_FONT_SIZE)
plt.tight_layout()
plt.savefig('/app/ufo_graphs/months_plot.png')
print("График месяцев сохранен: /app/ufo_graphs/months_plot.png")
plt.show()

# График 3: Типы объектов
shapes_type_count = pd.Series(df['shape'].values).value_counts()
shapes_type_count_keys, shapes_count_values = dict_sort(dict(shapes_type_count))
OBJECT_COUNT = min(len(shapes_type_count_keys), 15)

plt.figure(figsize=(12, 6))
plt.title('Типы объектов', fontsize=PLOT_LABEL_FONT_SIZE)
plt.bar(np.arange(OBJECT_COUNT), shapes_count_values[:OBJECT_COUNT], color=getColors(OBJECT_COUNT))
plt.xticks(np.arange(OBJECT_COUNT), shapes_type_count_keys[:OBJECT_COUNT], rotation=45, fontsize=PLOT_LABEL_FONT_SIZE)
plt.yticks(fontsize=PLOT_LABEL_FONT_SIZE)
plt.ylabel('Сколько раз видели', fontsize=PLOT_LABEL_FONT_SIZE)
plt.tight_layout()
plt.savefig('/app/ufo_graphs/shapes_plot.png')
print("График типов объектов сохранен: /app/ufo_graphs/shapes_plot.png")
plt.show()

# График 4: Среднее время появления
shapes_durations_dict = {}
for shape in shapes_type_count_keys[:OBJECT_COUNT]:
    try:
        dfs = df[['duration (seconds)', 'shape']].loc[df['shape'] == shape]
        dfs['duration (seconds)'] = pd.to_numeric(dfs['duration (seconds)'], errors='coerce')
        mean_duration = dfs['duration (seconds)'].mean()
        if not np.isnan(mean_duration):
            shapes_durations_dict[shape] = mean_duration / 3600.0
    except:
        continue

shapes_durations_dict_keys = list(shapes_durations_dict.keys())
shapes_durations_dict_values = list(shapes_durations_dict.values())

if shapes_durations_dict_keys:
    plt.figure(figsize=(12, 6))
    plt.title('Среднее время появление каждого объекта', fontsize=PLOT_LABEL_FONT_SIZE)
    plt.bar(np.arange(len(shapes_durations_dict_keys)), shapes_durations_dict_values, color=getColors(len(shapes_durations_dict_keys)))
    plt.xticks(np.arange(len(shapes_durations_dict_keys)), shapes_durations_dict_keys, rotation=45, fontsize=12)
    plt.ylabel('Среднее время появления в часах', fontsize=PLOT_LABEL_FONT_SIZE)
    plt.tight_layout()
    plt.savefig('/app/ufo_graphs/durations_plot.png')
    print("График времени появления сохранен: /app/ufo_graphs/durations_plot.png")
    plt.show()
else:
    print("Нет данных для графика времени появления")

print("Все графики успешно построены и сохранены в папку /app/ufo_graphs/")
