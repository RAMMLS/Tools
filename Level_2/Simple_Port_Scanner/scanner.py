from scapy.all import ARP, Ether, srp, wrpcap

def scan_net(target_ip):
  arp = ARP(pdst = target_ip)
  ether = Ether(dst = "ff:ff:ff:ff:ff:ff")
  packet = ether / arp
  result = srp(packet, timeout = 5, vebrose = 0) [0]
  clients = []

  for sent, received in results:
    clients.append({"ip": received.psrc, "mac": received.hwsrc})

  return clients

def save_to_pcap(clients, filename):
  packets = []
  for client in clients:
    arp = ARP(pdst = client["ip"], hwdst = client["mac"])
    ether = Ether(dst = client["mac"])
    packet = ether / arp
    packets.append(packet)

  wrpcap(filename, packets)
  print(f"Захваченные пакеты сохраняем в {filename}")

def print_scan_res(clients):
  print("Доступные устройства в сети: ")
  print("IP" + " "*18 + "MAC")
  for client in clients:
    print("{:16}    {}". format(client["ip"], client["mac"]))


if __name__ == "__main__":
  target+ip = input("Введите ваш IP адресс для сканирования сети")

clients = scan_network(target_ip)

print_scan_res(clients)

filename = input("Введите имя файла для сохранения захваченных пакетов: ")

save_to_pcap(clients, filename)
