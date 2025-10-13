# -*- coding: utf-8 -*-

import time
import urllib.request
from stem import Signal
from stem.control import Controller

class ConnectionManager:
    def __init__(self):
        self.new_ip = "0.0.0.0"
        self.old_ip = "0.0.0.0"
        self.new_identity()

    def _get_connection(self):
        """TOR new connection"""
        with Controller.from_port(port=9051) as controller:
            controller.authenticate(password="1234")
            controller.signal(Signal.NEWNYM)
            controller.close()

    def _set_url_proxy(self):
        """Request to URL through local proxy"""
        proxy_support = urllib.request.ProxyHandler({"http": "127.0.0.1:8118"})
        opener = urllib.request.build_opener(proxy_support)
        urllib.request.install_opener(opener)

    def request(self, url):
        """TOR communication through local proxy"""
        try:
            self._set_url_proxy()
            request = urllib.request.Request(url, None, {
                'User-Agent': "Mozilla/5.0 (X11; Linux x86_64) "
                              "AppleWebKit/535.11 (KHTML, like Gecko) "
                              "Ubuntu/10.10 Chromium/17.0.963.65 "
                              "Chrome/17.0.963.65 Safari/535.11"})
            response = urllib.request.urlopen(request)
            return response
        except urllib.error.HTTPError as e:
            return e

    def new_identity(self):
        """new connection with new IP"""
        if self.new_ip == "0.0.0.0":
            self._get_connection()
            self.new_ip = self.request("http://icanhazip.com/").read().decode('utf-8').strip()
        else:
            self.old_ip = self.new_ip
            self._get_connection()
            self.new_ip = self.request("http://icanhazip.com/").read().decode('utf-8').strip()

        seg = 0
        while self.old_ip == self.new_ip:
            time.sleep(5)
            seg += 5
            print(f"Ожидаем получения нового IP: {seg} секунд")
            self.new_ip = self.request("http://icanhazip.com/").read().decode('utf-8').strip()

        print(f"Новое подключение с IP: {self.new_ip}")
