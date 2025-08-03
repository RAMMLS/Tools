import socket

HOST = '127.0.0.1'
PORT = 65432

def rot13(text):
    result = ''
    for char in text:
        if 'a' <= char <= 'z':
            result += chr(((ord(char) - ord('a') + 13) % 26) + ord('a'))
        elif 'A' <= char <= 'Z':
            result += chr(((ord(char) - ord('A') + 13) % 26) + ord('A'))
        else:
            result += char
    return result

with socket.socket(socket.AF_INET, socket.SOCK_DGRAM) as sock:
    server_address = (HOST, PORT) # specify address to send to

    while True:
        message = input("Введите ваше сообщение (или 'exit' для выхода): ")
        if message.lower() == 'exit':
            break

        encoded_message = rot13(message)
        sock.sendto(encoded_message.encode('utf-8'), server_address) # use server_address

        try:
            sock.settimeout(5) # wait for response, prevent indefinite blocking
            data, addr = sock.recvfrom(1024)
            decoded_message = rot13(data.decode('utf-8'))
            print(f"Получено: {decoded_message}")

        except socket.timeout:
            print("Ответ от сервера не получен.")

