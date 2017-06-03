// stdafx.h : 標準のシステム インクルード ファイルのインクルード ファイル、または
// 参照回数が多く、かつあまり変更されない、プロジェクト専用のインクルード ファイル
// を記述します。
//

#pragma once
#pragma comment(lib, "Ws2_32.lib")

#include "targetver.h"

#include <stdio.h>
#include <tchar.h>
#include <ws2tcpip.h>
#include <iostream>
#include <string>
#include <winsock2.h>

// TODO: プログラムに必要な追加ヘッダーをここで参照してください。
class MySender
{
private:
	struct sockaddr_in addr;
	WSAData wsaData;
	SOCKET sock;
public:
	MySender() {
	}

	~MySender()//disconnect忘れ防止
	{
		closesocket(sock);
		WSACleanup();
	}

	void MySender::connect(char* address, int port)
	{
		WSAStartup(MAKEWORD(2, 0), &wsaData);
		addr.sin_family = AF_INET;
		addr.sin_port = htons(port);
		inet_pton(AF_INET, address, &addr.sin_addr.S_un.S_addr);
		sock = socket(AF_INET, SOCK_DGRAM, 0);
	}

	void MySender::disconnect()
	{
		closesocket(sock);
		WSACleanup();
	}
	void MySender::send(char* message) {
		sendto(sock, message, strlen(message), 0, (struct sockaddr *)&addr, sizeof(addr));
	}
};



class MyReceiver
{
private:
	WSAData wsaData;
	SOCKET sock;
	struct sockaddr_in addr;
	char buf[2048];			//2048文字まで
public:

	void connect(int port)
	{
		WSAStartup(MAKEWORD(2, 0), &wsaData);

		sock = socket(AF_INET, SOCK_DGRAM, 0);

		addr.sin_family = AF_INET;
		addr.sin_port = htons(port);
		addr.sin_addr.S_un.S_addr = INADDR_ANY;

		bind(sock, (struct sockaddr *)&addr, sizeof(addr));

		memset(buf, 0, sizeof(buf));//bufの初期化
	}
	void disconnect()
	{
		closesocket(sock);
		WSACleanup();
	}
	char* recvwait() {
		recv(sock, buf, sizeof(buf), 0);//受信待ち
		return buf;
	}

};