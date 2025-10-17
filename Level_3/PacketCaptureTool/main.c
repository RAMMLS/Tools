#include <stdio.h>


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
