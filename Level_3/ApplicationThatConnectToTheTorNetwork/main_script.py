from ConnectionManager import ConnectionManager

# Пример использования
cm = ConnectionManager()
for j in range(3):
    for i in range(3):
        response = cm.request("http://icanhazip.com/")
        if hasattr(response, 'read'):
            print("\t\t" + response.read().decode('utf-8').strip())
    cm.new_identity()
