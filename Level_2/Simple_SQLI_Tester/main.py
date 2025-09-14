import sqlite3 
#Подключение к бд 
conn = sqlite3.connect(':memory:')
cursor = conn.cursor()

#Создание таблицы и добавление тестовых данных
cursor.execute("CREATE TABLE users (id INT, name TEXT)")
cursor.execute("INSERT INTO users VALUES (1, 'Alice')")
cursor.execute("INSERT INTO users VALUES (2, 'Bob')")
conn.commit()

#Тестовый запрос
cursor.execute("SELECT name FROM users WHERE id = 1")
result = cursor.fetchone()

#Проверка результата 
expected_result = ('Alice')
if result == expected_result:
    print("Тест пройден успешно!")
else:
    print(f"Тест не пройден. Ожидалось: {expected_result}, Получено: {result}")

#Закрытие соединения
conn.close()
