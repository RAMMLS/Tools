import socket

serverAddresPort = ("127.0.0.1", 65432)

bytesToSend = b"Мы написали письмо"

#Создание TCP/IP сокета
TCPServerSocket.bind(serverAddresPort)
print("Сервер прослушивается")

#Прослушивание входящих сообщений
TCPServerSocket.listen(10)
msg, address = TCPServerSocket.accept()

while 1:
    datafromCLient = msg.recv(1024)
    msg.sendall(datafromClient) 
