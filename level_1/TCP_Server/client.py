import socket

localIP, localPort = "127.0.0.1", 65432

#Создаем TCP/IP сокет

TCPclientSocket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)

TCPclientSocket.connect((localIP, localPort))

clientMsg = input("Введите сообщение для сервера: ")
data = bytes(clientMsg, "utf-8")

#Отправляем сообщение серверу используя TCP сокет
print("Отправляем сообщение {0} порт {1}".format(localIP, localPort))
TCPclientSocket.sendall(data)

#Ответ от сервера
dataFromServer = str(TCPclientSocket.recv(1024))
print("Сообщение, полученное от сервера: ", str(dataFromServer))
