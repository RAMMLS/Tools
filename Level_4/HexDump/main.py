import sys
import argparse

def is_printable(byte):
    """Проверяет, является ли байт печатным ASCII-символом"""
    return 32 <= byte <= 126

def hexdump(file_path, bytes_per_line=16, show_ascii=True):
    try:
        with open(file_path, 'rb') as file:
            offset = 0
            while True:
                chunk = file.read(bytes_per_line)
                if not chunk:
                    break
                
                # Hex-отображение
                hex_part = ' '.join(f'{b:02x}' for b in chunk)
                
                # ASCII-отображение
                ascii_part = ''.join(chr(b) if is_printable(b) else '.' for b in chunk)
                
                # Форматирование строки
                if show_ascii:
                    print(f'{offset:08x}: {hex_part:<{bytes_per_line*3}}  {ascii_part}')
                else:
                    print(f'{offset:08x}: {hex_part}')
                
                offset += len(chunk)
                
    except Exception as e:
        print(f"Ошибка: {e}", file=sys.stderr)

def main():
    parser = argparse.ArgumentParser(description='Hexdump tool by Senjour')
    parser.add_argument('file', help='Путь к файлу')
    parser.add_argument('-n', '--length', type=int, help='Количество байт для вывода')
    parser.add_argument('-s', '--skip', type=int, help='Пропустить первые N байт')
    parser.add_argument('--no-ascii', action='store_true', help='Не показывать ASCII-представление')
    
    args = parser.parse_args()
    
    # Здесь можно добавить обработку -s и -n (для краткости опущена)
    hexdump(args.file, show_ascii=not args.no_ascii)

if __name__ == '__main__':
    main()
