import socket
import threading

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

def handle_client(sock, addr, known_clients):
    """Обрабатывает общение с одним клиентом."""
    try:
        while True:
            data, addr = sock.recvfrom(1024)
            if not data:
                break  # Клиент ничего не отправил

            message = data.decode('utf-8')
            decoded_message = rot13(message)
            print(f"Получено от {addr}: {decoded_message}")

            encoded_response = rot13(decoded_message)
            sock.sendto(encoded_response.encode('utf-8'), addr)

    except Exception as e:
        print(f"Ошибка при обработке клиента {addr}: {e}")
    finally:
        print(f"Завершено общение с {addr}")
        known_clients.remove(addr) # remove client from known clients when done

def main():
    """Основная функция для запуска UDP чат-сервера."""
    sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
    known_clients = set() # track connected clients

    try:
        sock.bind((HOST, PORT))
        print(f"Слушаем на {HOST}:{PORT}")

        while True:
            data, addr = sock.recvfrom(1024)

            if addr not in known_clients:
                print(f"Новый клиент: {addr}. Запускаем поток.")
                known_clients.add(addr)
                client_thread = threading.Thread(target=handle_client, args=(sock, addr, known_clients)) # pass known clients
                client_thread.daemon = True
                client_thread.start()

            else:
                # Existing client - already handled by a thread
                pass # the handle_client function will receive the data

    except OSError as e:
        print(f"Ошибка при привязке к сокету: {e}")
    except KeyboardInterrupt:
        print("Завершение работы сервера...")
    finally:
        sock.close()
        print("Сервер остановлен.")

if __name__ == "__main__":
    main()

