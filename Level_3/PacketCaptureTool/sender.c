#include <stdio.h>
#include <libpcap.h>

u_char* learn_IP()
{
	addrinfo hnts, *pRes;
	char hostName[1024];
	u_char* ip_addr = NULL;
	if (gethostname(hostName, sizeof hostName) == 0)
	{
	memset(&hnts, 0, sizeof hnts);
	hnts.ai_family = AF_INET; hnts.ai_socktype = SOCK_DGRAM; hnts.ai_flags = AI_PASSIVE;
	if (getaddrinfo(hostName, NULL, &hnts, &pRes) == 0)
	{
	struct addrinfo* res;
	char buffer[INET_ADDRSTRLEN];
	for (res = pRes; (res != NULL) && (res->ai_family != AF_INET); res = res->ai_next);
	ip_addr = (u_char* )inet_ntop(AF_INET, &((struct sockaddr_in *)res->ai_addr)->sin_addr, buffer, INET_ADDRSTRLEN);
	freeaddrinfo(pResults);
	}
	WSACleanup();
	}
	return ip_addr;
}



u_char *learn_MAC() {
  IP_ADAPTER_INFO ip_ainf[128];
  PIP_ADAPTER_INFO pip_ainf = ip_ainf;
  u_long bufLen = sizeof(ip_ainf);
  GetAdaptersInfo(ip_ainf, &bufLen);
  u_char* mac_addr = pip_ainf -> Address;

  return mac_addr;
}



IPAddr DestIP = 0, ScrIP = 0;
static u_long MacAddr[2];
u_long PhysAddrLen;
ScrIpAddr = inet_addr(ScriptString);  // IP адрес отправителя
DestIPAddr = inet_addr(destIpString); // IP адрес маршрутизатора
memset (&MacAddr, 0xff, sizeof(MacAddr)); // Широковещательная рассылка 
if (SendAddr (DestIPAddr, ScrIpAddr, &MacAddr, &PhysAddrLen) == NO_ERROR) {
  *bPhysAddr = (BYTE *) & MacAddr; // Искомый MAC адрес
}

// Определяем интерфейс
pcap_if_t *alldevs, *dev;
char errbuf[PCAP_ERRBUF_SIZE];
int inum, i =0;
pcap_findalldevs(&alldevs, errbuf);
scanf_s("%dev", &inum);
for (dev = alldevs, i = 0; i < inum -1; dev = dev -> next, i++)
  pcap_t *adhandle;
adhandle = pcap_open_line(dev -> name, 65535, 0, 1000, errbuf);


// Собираем воедино пакет для отправки 
u_short checksum(u_char *buffer, int size) {
  u_long chksum = 0;
  while (size > 1) {
    chksum += *buffer++;
    size -= sizeof(u_short);
  }

  if (size) 
    chksum += *(u_char*)buffer;
  chksum = (chksum >> 16) + (chksum &0xffff);
  chksum += (chksum >> 16);
  
  return (u_short)(~chksum)
}

pcap_sendpacket(adhandle, packet, 43);
